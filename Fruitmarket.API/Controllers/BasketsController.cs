using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BasketsController(IBasketService baskets) : ControllerBase
{
    /// <summary>Gets the curated storefront fruit baskets/hampers.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BasketDto>>> Get(CancellationToken ct) => Ok(await baskets.GetAsync(ct));

    /// <summary>Gets a single basket by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BasketDto>> GetById(Guid id, CancellationToken ct) => Ok(await baskets.GetByIdAsync(id, ct));
}
