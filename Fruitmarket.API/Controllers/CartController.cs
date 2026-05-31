using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CartController(ICartService cart) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CartDto>> Get(CancellationToken ct) => Ok(await cart.GetAsync(ct));

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem(AddCartItemRequest request, CancellationToken ct) => Ok(await cart.AddItemAsync(request, ct));

    [HttpPut("items/{itemId:guid}")]
    public async Task<ActionResult<CartDto>> UpdateItem(Guid itemId, UpdateCartItemRequest request, CancellationToken ct) => Ok(await cart.UpdateItemAsync(itemId, request, ct));

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid itemId, CancellationToken ct)
    {
        await cart.RemoveItemAsync(itemId, ct);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        await cart.ClearAsync(ct);
        return NoContent();
    }
}
