using Fruitmarket.Application.Abstractions;
using Fruitmarket.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public sealed class InventoryController(IUnitOfWork uow) : ControllerBase
{
    [HttpPatch("products/{productId:guid}/stock")]
    public async Task<IActionResult> UpdateStock(Guid productId, [FromBody] int quantity, CancellationToken ct)
    {
        var product = await uow.Products.GetByIdAsync(productId, ct) ?? throw new ApiException("Product not found", 404);
        product.StockQuantity = Math.Max(0, quantity);
        await uow.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("out-of-stock")]
    public IActionResult OutOfStock() => Ok(uow.Products.Query().Where(x => x.StockQuantity <= 0).Select(x => new { x.Id, x.NameEn, x.Slug, x.StockQuantity }).ToList());
}
