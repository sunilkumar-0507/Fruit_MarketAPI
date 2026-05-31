using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CouponsController(ICouponService coupons) : ControllerBase
{
    /// <summary>Validates a coupon code.</summary>
    [HttpPost("validate")]
    public async Task<ActionResult<CouponValidationResponse>> Validate([FromBody] string code, CancellationToken ct)
        => Ok(await coupons.ValidateAsync(code, ct));
}
