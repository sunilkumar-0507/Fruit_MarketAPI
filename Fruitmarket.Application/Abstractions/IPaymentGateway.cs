namespace Fruitmarket.Application.Abstractions;

/// <summary>Data needed to open a payment order with the gateway (Cashfree PG Orders API v3).</summary>
public sealed record PaymentGatewayCreate(
    string OrderId,
    decimal Amount,
    string CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone);

/// <summary>Result of creating a gateway order — the session id the JS SDK needs.</summary>
public sealed record PaymentGatewayOrder(string CashfreeOrderId, string PaymentSessionId);

/// <summary>Current state of a gateway order plus the best transaction id we found.</summary>
public sealed record PaymentGatewayResult(string OrderStatus, string? TransactionId);

/// <summary>Low-level Cashfree HTTP boundary. Implemented in Infrastructure.</summary>
public interface IPaymentGateway
{
    /// <summary>True only when Cashfree credentials are configured and enabled.</summary>
    bool IsConfigured { get; }

    Task<PaymentGatewayOrder> CreateOrderAsync(PaymentGatewayCreate request, CancellationToken ct = default);

    /// <summary>Fetches the order status (PAID/ACTIVE/EXPIRED) and the successful payment's transaction id, if any.</summary>
    Task<PaymentGatewayResult> GetOrderResultAsync(string cashfreeOrderId, CancellationToken ct = default);

    /// <summary>Verifies a Cashfree webhook signature: Base64(HMAC-SHA256(timestamp + rawBody, SecretKey)).</summary>
    bool VerifyWebhookSignature(string rawBody, string signature, string timestamp);
}
