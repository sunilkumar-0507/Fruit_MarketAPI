using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

/// <summary>Cashfree online-payment endpoints: create session, verify, and the provider webhook.</summary>
[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(IPaymentService payments) : ControllerBase
{
    /// <summary>Creates a Cashfree payment session for an existing online order. Returns the payment_session_id for the JS SDK.</summary>
    [Authorize]
    [HttpPost("{orderId:guid}/session")]
    public async Task<ActionResult<PaymentSessionDto>> CreateSession(Guid orderId, CreatePaymentSessionRequest request, CancellationToken ct)
        => Ok(await payments.CreateSessionAsync(orderId, request, ct));

    /// <summary>Verifies payment with Cashfree after checkout returns, persists the transaction id, and returns the updated order.</summary>
    [Authorize]
    [HttpPost("{orderId:guid}/verify")]
    public async Task<ActionResult<OrderDto>> Verify(Guid orderId, CancellationToken ct)
        => Ok(await payments.VerifyAsync(orderId, ct));

    /// <summary>Cashfree server-to-server webhook. Signature is verified before any DB write.</summary>
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync(ct);
        var signature = Request.Headers["x-webhook-signature"].ToString();
        var timestamp = Request.Headers["x-webhook-timestamp"].ToString();
        await payments.ProcessWebhookAsync(rawBody, signature, timestamp, ct);
        return Ok();
    }
}
