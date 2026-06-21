using System.Net.Http.Headers;
using Fruitmarket.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fruitmarket.Infrastructure.Services;

public sealed class SmsOptions
{
    /// <summary>When false (or the API key is missing), the OTP is logged instead of sent — handy in dev.</summary>
    public bool Enabled { get; set; }
    /// <summary>SMS provider: "Fast2SMS" (default) or "MSG91".</summary>
    public string Provider { get; set; } = "Fast2SMS";
    public string ApiKey { get; set; } = string.Empty;
    /// <summary>DLT-approved sender id (MSG91) — not required for the Fast2SMS OTP route.</summary>
    public string SenderId { get; set; } = string.Empty;
    /// <summary>DLT template id used by MSG91's OTP/flow API.</summary>
    public string TemplateId { get; set; } = string.Empty;
}

/// <summary>
/// HTTP boundary to the SMS gateway. Registered as a typed HttpClient. Supports the Fast2SMS
/// "otp" route and MSG91's OTP API; both are common, low-friction choices for Indian numbers.
/// Failures are surfaced to the caller, which treats delivery as best-effort.
/// </summary>
public sealed class SmsSender(HttpClient http, IOptions<SmsOptions> options, ILogger<SmsSender> logger) : ISmsSender
{
    private readonly SmsOptions _options = options.Value;

    public async Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            // Dev fallback: don't send, just log so the flow is testable without a real gateway.
            logger.LogWarning("SMS sending disabled — OTP for {Phone} is {Otp}.", phoneNumber, otp);
            return;
        }

        var response = string.Equals(_options.Provider, "MSG91", StringComparison.OrdinalIgnoreCase)
            ? await SendViaMsg91Async(phoneNumber, otp, ct)
            : await SendViaFast2SmsAsync(phoneNumber, otp, ct);

        var payload = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"SMS gateway returned {(int)response.StatusCode}: {payload}");

        logger.LogInformation("OTP SMS dispatched to {Phone} via {Provider}.", phoneNumber, _options.Provider);
    }

    // Fast2SMS OTP route: GET /dev/bulkV2?route=otp&variables_values={otp}&numbers={10-digit}
    // Auth is passed in the `authorization` header. Sends a standard "{otp} is your OTP" message.
    private Task<HttpResponseMessage> SendViaFast2SmsAsync(string phoneNumber, string otp, CancellationToken ct)
    {
        var url = $"https://www.fast2sms.com/dev/bulkV2?route=otp&variables_values={Uri.EscapeDataString(otp)}&flash=0&numbers={Uri.EscapeDataString(phoneNumber)}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("authorization", _options.ApiKey);
        return http.SendAsync(request, ct);
    }

    // MSG91 OTP API: POST /api/v5/otp with the template id; the OTP is sent as a template variable.
    private Task<HttpResponseMessage> SendViaMsg91Async(string phoneNumber, string otp, CancellationToken ct)
    {
        var url = $"https://control.msg91.com/api/v5/otp?template_id={Uri.EscapeDataString(_options.TemplateId)}&mobile=91{Uri.EscapeDataString(phoneNumber)}&otp={Uri.EscapeDataString(otp)}";
        if (!string.IsNullOrWhiteSpace(_options.SenderId)) url += $"&sender={Uri.EscapeDataString(_options.SenderId)}";
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent("{}", System.Text.Encoding.UTF8, new MediaTypeHeaderValue("application/json")),
        };
        request.Headers.TryAddWithoutValidation("authkey", _options.ApiKey);
        return http.SendAsync(request, ct);
    }
}
