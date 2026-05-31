using Fruitmarket.Application.Common;
using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(IProductService products, IReviewService reviews) : ControllerBase
{
    /// <summary>Gets paged products with filtering and sorting.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> Get([FromQuery] ProductQuery query, CancellationToken ct) => Ok(await products.GetAsync(query, ct));

    /// <summary>Searches products.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<ProductDto>>> Search([FromQuery] string q, CancellationToken ct) => Ok(await products.GetAsync(new ProductQuery(q, null, null, null, null, false), ct));

    /// <summary>Gets a product by slug.</summary>
    [HttpGet("{slug}")]
    public async Task<ActionResult<ProductDto>> GetBySlug(string slug, CancellationToken ct) => Ok(await products.GetBySlugAsync(slug, ct));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(ProductUpsertRequest request, CancellationToken ct) => Created(string.Empty, await products.CreateAsync(request, ct));

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, ProductUpsertRequest request, CancellationToken ct) => Ok(await products.UpdateAsync(id, request, ct));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await products.DeleteAsync(id, ct);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{slug}/reviews")]
    public async Task<ActionResult<ReviewDto>> AddReview(string slug, ReviewRequest request, CancellationToken ct) => Created(string.Empty, await reviews.AddAsync(slug, request, ct));

    [HttpGet("{slug}/reviews")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetReviews(string slug, CancellationToken ct) => Ok(await reviews.GetByProductAsync(slug, ct));
}
