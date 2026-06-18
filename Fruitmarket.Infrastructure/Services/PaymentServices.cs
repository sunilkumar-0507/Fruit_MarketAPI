using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Fruitmarket.Application.Abstractions;
using Fruitmarket.Application.Common;
using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Fruitmarket.Domain.Entities;
using Fruitmarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fruitmarket.Infrastructure.Services;

public sealed class CashfreeOptions
{
    /// <summary>When false (or credentials missing), online payment is rejected with a clear error.</summary>
    public bool Enabled { get; set; }
    /// <summary>"sandbox" (default) or "production" — selects the Cashfree base URL.</summary>
    public string Environment { get; set; } = "sandbox";
    public string AppId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "2023-08-01";
    /// <summary>Where Cashfree returns the browser if a payment method forces a full-page redirect.
    /// The seamless modal flow normally stays on-site; the webhook settles redirect cases server-side.</summary>
    public string ReturnUrl { get; set; } = "https://tenkasifresh.in/orders";

    public string BaseUrl => string.Equals(Environment, "production", StringComparison.OrdinalIgnoreCase)
        ? "https://api.cashfree.com"
        : "https://sandbox.cashfree.com";
}

/// <summary>HTTP boundary to the Cashfree PG Orders API v3. Registered as a typed HttpClient.</summary>
public sealed class CashfreeGateway(HttpClient http, IOptions<CashfreeOptions> options, ILogger<CashfreeGateway> logger) : IPaymentGateway
{
    private readonly CashfreeOptions _options = options.Value;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public bool IsConfigured => _options.Enabled
        && !string.IsNullOrWhiteSpace(_options.AppId)
        && !string.IsNullOrWhiteSpace(_options.SecretKey);

    public async Task<PaymentGatewayOrder> CreateOrderAsync(PaymentGatewayCreate request, CancellationToken ct = default)
    {
        EnsureConfigured();

        var body = new
        {
            order_id = request.OrderId,
            order_amount = decimal.Round(request.Amount, 2),
            order_currency = "INR",
            customer_details = new
            {
                customer_id = request.CustomerId,
                customer_name = request.CustomerName,
                customer_email = request.CustomerEmail,
                customer_phone = request.CustomerPhone,
            },
            order_meta = new { return_url = _options.ReturnUrl },
        };

        using var req = BuildRequest(HttpMethod.Post, "/pg/orders");
        req.Content = JsonContent.Create(body, options: JsonOpts);
        using var res = await http.SendAsync(req, ct);
        var json = await res.Content.ReadAsStringAsync(ct);
        if (!res.IsSuccessStatusCode)
        {
            logger.LogError("Cashfree create-order failed ({Status}): {Body}", (int)res.StatusCode, json);
            throw new ApiException(ExtractMessage(json) ?? "Payment provider rejected the order.", 502);
        }

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var sessionId = root.TryGetProperty("payment_session_id", out var s) ? s.GetString() : null;
        var cfOrderId = root.TryGetProperty("order_id", out var o) ? o.GetString() : request.OrderId;
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ApiException("Payment provider did not return a payment session.", 502);

        return new PaymentGatewayOrder(cfOrderId ?? request.OrderId, sessionId);
    }

    public async Task<PaymentGatewayResult> GetOrderResultAsync(string cashfreeOrderId, CancellationToken ct = default)
    {
        EnsureConfigured();

        // Order status (PAID / ACTIVE / EXPIRED / TERMINATED).
        using var orderReq = BuildRequest(HttpMethod.Get, $"/pg/orders/{cashfreeOrderId}");
        using var orderRes = await http.SendAsync(orderReq, ct);
        var orderJson = await orderRes.Content.ReadAsStringAsync(ct);
        if (!orderRes.IsSuccessStatusCode)
        {
            logger.LogError("Cashfree get-order failed ({Status}): {Body}", (int)orderRes.StatusCode, orderJson);
            throw new ApiException("Could not verify payment with the provider.", 502);
        }
        using var orderDoc = JsonDocument.Parse(orderJson);
        var orderStatus = orderDoc.RootElement.TryGetProperty("order_status", out var os) ? os.GetString() ?? "UNKNOWN" : "UNKNOWN";

        // Successful payment's transaction id (cf_payment_id).
        string? transactionId = null;
        using var payReq = BuildRequest(HttpMethod.Get, $"/pg/orders/{cashfreeOrderId}/payments");
        using var payRes = await http.SendAsync(payReq, ct);
        var payJson = await payRes.Content.ReadAsStringAsync(ct);
        if (payRes.IsSuccessStatusCode)
        {
            using var payDoc = JsonDocument.Parse(payJson);
            if (payDoc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in payDoc.RootElement.EnumerateArray())
                {
                    var status = p.TryGetProperty("payment_status", out var ps) ? ps.GetString() : null;
                    if (string.Equals(status, "SUCCESS", StringComparison.OrdinalIgnoreCase) && p.TryGetProperty("cf_payment_id", out var cf))
                    {
                        transactionId = cf.ValueKind == JsonValueKind.Number ? cf.GetRawText() : cf.GetString();
                        break;
                    }
                }
            }
        }

        return new PaymentGatewayResult(orderStatus, transactionId);
    }

    public bool VerifyWebhookSignature(string rawBody, string signature, string timestamp)
    {
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp) || string.IsNullOrWhiteSpace(_options.SecretKey))
            return false;
        var payload = timestamp + rawBody;
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.SecretKey));
        var computed = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(computed), Encoding.UTF8.GetBytes(signature));
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string path)
    {
        var req = new HttpRequestMessage(method, _options.BaseUrl + path);
        req.Headers.Add("x-client-id", _options.AppId);
        req.Headers.Add("x-client-secret", _options.SecretKey);
        req.Headers.Add("x-api-version", _options.ApiVersion);
        return req;
    }

    private void EnsureConfigured()
    {
        if (!IsConfigured)
            throw new ApiException("Online payment is not configured. Please use Cash on Delivery.", 503);
    }

    private static string? ExtractMessage(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("message", out var m) ? m.GetString() : null;
        }
        catch { return null; }
    }
}

public sealed class PaymentService(IUnitOfWork uow, IPaymentGateway gateway, IOrderService orders, ICurrentUserService currentUser, ILogger<PaymentService> logger) : IPaymentService
{
    public async Task<PaymentSessionDto> CreateSessionAsync(Guid orderId, CreatePaymentSessionRequest request, CancellationToken ct)
    {
        var order = await uow.Orders.Query().Include(x => x.User).FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == currentUser.UserId, ct)
            ?? throw new ApiException("Order not found", 404);
        if (order.PaymentMethod != PaymentMethod.Online)
            throw new ApiException("This order is not an online-payment order.", 400);
        if (order.PaymentStatus == PaymentStatus.Paid)
            throw new ApiException("This order is already paid.", 409);
        if (order.Total <= 0)
            throw new ApiException("Order total must be greater than zero for online payment.", 400);

        var phone = SanitizePhone(request.CustomerPhone) ?? SanitizePhone(order.User?.PhoneNumber)
            ?? throw new ApiException("A valid 10-digit phone number is required for online payment.", 400);
        var name = !string.IsNullOrWhiteSpace(request.CustomerName) ? request.CustomerName!
            : !string.IsNullOrWhiteSpace(order.User?.FullName) ? order.User!.FullName : "Customer";
        var email = !string.IsNullOrWhiteSpace(order.User?.Email) ? order.User!.Email! : "customer@tenkasifresh.in";

        // Fresh provider order id each attempt so abandoned-then-retried payments don't collide.
        var cashfreeOrderId = $"tf_{Guid.NewGuid():N}";
        var session = await gateway.CreateOrderAsync(
            new PaymentGatewayCreate(cashfreeOrderId, order.Total, order.UserId.ToString(), name, email, phone), ct);

        order.PaymentProviderOrderId = session.CashfreeOrderId;
        await uow.SaveChangesAsync(ct);

        return new PaymentSessionDto(order.Id.ToString(), session.PaymentSessionId, session.CashfreeOrderId, order.Total);
    }

    public async Task<OrderDto> VerifyAsync(Guid orderId, CancellationToken ct)
    {
        var order = await uow.Orders.Query().FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == currentUser.UserId, ct)
            ?? throw new ApiException("Order not found", 404);
        if (string.IsNullOrWhiteSpace(order.PaymentProviderOrderId))
            throw new ApiException("No payment was started for this order.", 400);

        if (order.PaymentStatus != PaymentStatus.Paid)
        {
            var result = await gateway.GetOrderResultAsync(order.PaymentProviderOrderId, ct);
            ApplyResult(order, result.OrderStatus, result.TransactionId);
            await uow.SaveChangesAsync(ct);
        }

        return await orders.GetByIdAsync(orderId, ct);
    }

    public async Task ProcessWebhookAsync(string rawBody, string signature, string timestamp, CancellationToken ct)
    {
        if (!gateway.VerifyWebhookSignature(rawBody, signature, timestamp))
        {
            logger.LogWarning("Rejected Cashfree webhook with invalid signature.");
            throw new ApiException("Invalid webhook signature.", 401);
        }

        string? cfOrderId, paymentStatus, transactionId;
        try
        {
            using var doc = JsonDocument.Parse(rawBody);
            var data = doc.RootElement.GetProperty("data");
            cfOrderId = data.GetProperty("order").GetProperty("order_id").GetString();
            var payment = data.TryGetProperty("payment", out var p) ? p : default;
            paymentStatus = payment.ValueKind == JsonValueKind.Object && payment.TryGetProperty("payment_status", out var ps) ? ps.GetString() : null;
            transactionId = payment.ValueKind == JsonValueKind.Object && payment.TryGetProperty("cf_payment_id", out var cf)
                ? (cf.ValueKind == JsonValueKind.Number ? cf.GetRawText() : cf.GetString())
                : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not parse Cashfree webhook body.");
            return; // Acknowledge — a malformed body should not be retried forever.
        }

        if (string.IsNullOrWhiteSpace(cfOrderId)) return;
        var order = await uow.Orders.Query().FirstOrDefaultAsync(x => x.PaymentProviderOrderId == cfOrderId, ct);
        if (order is null)
        {
            logger.LogWarning("Cashfree webhook for unknown provider order id {CfOrderId}.", cfOrderId);
            return;
        }
        if (order.PaymentStatus == PaymentStatus.Paid) return; // Already settled.

        // Map a payment-level status if present, else fall back to a provider order lookup.
        if (string.Equals(paymentStatus, "SUCCESS", StringComparison.OrdinalIgnoreCase))
        {
            ApplyResult(order, "PAID", transactionId);
        }
        else
        {
            var result = await gateway.GetOrderResultAsync(cfOrderId, ct);
            ApplyResult(order, result.OrderStatus, result.TransactionId ?? transactionId);
        }
        await uow.SaveChangesAsync(ct);
    }

    private static void ApplyResult(Order order, string orderStatus, string? transactionId)
    {
        if (string.Equals(orderStatus, "PAID", StringComparison.OrdinalIgnoreCase))
        {
            order.PaymentStatus = PaymentStatus.Paid;
            order.PaymentTransactionId = transactionId ?? order.PaymentTransactionId;
            order.PaidAtUtc = DateTime.UtcNow;
            if (order.Status == OrderStatus.Pending) order.Status = OrderStatus.Confirmed;
        }
        else if (string.Equals(orderStatus, "EXPIRED", StringComparison.OrdinalIgnoreCase)
              || string.Equals(orderStatus, "TERMINATED", StringComparison.OrdinalIgnoreCase))
        {
            order.PaymentStatus = PaymentStatus.Failed;
        }
        // "ACTIVE" → still pending; leave as-is.
    }

    /// <summary>Returns the last 10 digits of a phone, or null if there aren't enough digits.</summary>
    private static string? SanitizePhone(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var digits = new string(raw.Where(char.IsDigit).ToArray());
        return digits.Length >= 10 ? digits[^10..] : null;
    }
}
