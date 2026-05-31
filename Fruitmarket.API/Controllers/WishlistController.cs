using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class WishlistController(IWishlistService wishlist) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> Get(CancellationToken ct) => Ok(await wishlist.GetAsync(ct));

    [HttpPost("{productId:guid}")]
    public async Task<IActionResult> Add(Guid productId, CancellationToken ct)
    {
        await wishlist.AddAsync(productId, ct);
        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Remove(Guid productId, CancellationToken ct)
    {
        await wishlist.RemoveAsync(productId, ct);
        return NoContent();
    }
}
