using Fruitmarket.Application.Common;
using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

/// <summary>Admin-only management endpoints.</summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public sealed class AdminController(
    IProductService products,
    ICategoryService categories,
    IOrderService orders,
    ICouponService coupons,
    IFarmerService farmers,
    IBasketService baskets,
    IAuthService auth) : ControllerBase
{
    // ── Users ─────────────────────────────────────────────────────

    /// <summary>Gets all users with their roles (paged). New registrations are "Customer"; change to "Admin" in the database.</summary>
    [HttpGet("users")]
    public async Task<ActionResult<PagedResult<UserDto>>> Users([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await auth.GetUsersAsync(pageNumber, pageSize, ct));

    // ── Products ─────────────────────────────────────────────────

    /// <summary>Gets all products (admin view).</summary>
    [HttpGet("products")]
    public async Task<ActionResult<PagedResult<ProductDto>>> Products([FromQuery] ProductQuery query, CancellationToken ct)
        => Ok(await products.GetAsync(query with { IncludeInactive = true }, ct));

    /// <summary>Creates a product.</summary>
    [HttpPost("products")]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductUpsertRequest request, CancellationToken ct)
        => Created(string.Empty, await products.CreateAsync(request, ct));

    /// <summary>Updates a product.</summary>
    [HttpPut("products/{id:guid}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, ProductUpsertRequest request, CancellationToken ct)
        => Ok(await products.UpdateAsync(id, request, ct));

    /// <summary>Soft-deletes a product.</summary>
    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        await products.DeleteAsync(id, ct);
        return NoContent();
    }

    // ── Categories ────────────────────────────────────────────────

    /// <summary>Gets all categories.</summary>
    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> Categories(CancellationToken ct)
        => Ok(await categories.GetAsync(ct));

    /// <summary>Creates a category.</summary>
    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryUpsertRequest request, CancellationToken ct)
        => Created(string.Empty, await categories.CreateAsync(request, ct));

    /// <summary>Updates a category.</summary>
    [HttpPut("categories/{id:guid}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(Guid id, CategoryUpsertRequest request, CancellationToken ct)
        => Ok(await categories.UpdateAsync(id, request, ct));

    /// <summary>Deletes a category.</summary>
    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken ct)
    {
        await categories.DeleteAsync(id, ct);
        return NoContent();
    }

    // ── Orders ────────────────────────────────────────────────────

    /// <summary>Gets all orders (paged).</summary>
    [HttpGet("orders")]
    public async Task<ActionResult<PagedResult<OrderDto>>> Orders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await orders.GetAllAsync(pageNumber, pageSize, ct));

    /// <summary>Updates order status.</summary>
    [HttpPatch("orders/{id:guid}/status")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(Guid id, UpdateOrderStatusRequest request, CancellationToken ct)
        => Ok(await orders.UpdateStatusAsync(id, request.Status, ct));

    // ── Coupons ───────────────────────────────────────────────────

    /// <summary>Gets all coupons.</summary>
    [HttpGet("coupons")]
    public async Task<ActionResult<IReadOnlyList<CouponDto>>> GetCoupons(CancellationToken ct)
        => Ok(await coupons.GetAsync(ct));

    /// <summary>Gets a coupon by ID.</summary>
    [HttpGet("coupons/{id:guid}")]
    public async Task<ActionResult<CouponDto>> GetCoupon(Guid id, CancellationToken ct)
        => Ok(await coupons.GetByIdAsync(id, ct));

    /// <summary>Creates a coupon.</summary>
    [HttpPost("coupons")]
    public async Task<ActionResult<CouponDto>> CreateCoupon(CouponUpsertRequest request, CancellationToken ct)
        => Created(string.Empty, await coupons.CreateAsync(request, ct));

    /// <summary>Updates a coupon.</summary>
    [HttpPut("coupons/{id:guid}")]
    public async Task<ActionResult<CouponDto>> UpdateCoupon(Guid id, CouponUpsertRequest request, CancellationToken ct)
        => Ok(await coupons.UpdateAsync(id, request, ct));

    /// <summary>Deletes a coupon.</summary>
    [HttpDelete("coupons/{id:guid}")]
    public async Task<IActionResult> DeleteCoupon(Guid id, CancellationToken ct)
    {
        await coupons.DeleteAsync(id, ct);
        return NoContent();
    }

    // ── Farmers ───────────────────────────────────────────────────

    /// <summary>Gets all partner farmers.</summary>
    [HttpGet("farmers")]
    public async Task<ActionResult<IReadOnlyList<FarmerDto>>> GetFarmers(CancellationToken ct)
        => Ok(await farmers.GetAsync(ct));

    /// <summary>Gets a farmer by ID.</summary>
    [HttpGet("farmers/{id:guid}")]
    public async Task<ActionResult<FarmerDto>> GetFarmer(Guid id, CancellationToken ct)
        => Ok(await farmers.GetByIdAsync(id, ct));

    /// <summary>Creates a farmer.</summary>
    [HttpPost("farmers")]
    public async Task<ActionResult<FarmerDto>> CreateFarmer(FarmerUpsertRequest request, CancellationToken ct)
        => Created(string.Empty, await farmers.CreateAsync(request, ct));

    /// <summary>Updates a farmer.</summary>
    [HttpPut("farmers/{id:guid}")]
    public async Task<ActionResult<FarmerDto>> UpdateFarmer(Guid id, FarmerUpsertRequest request, CancellationToken ct)
        => Ok(await farmers.UpdateAsync(id, request, ct));

    /// <summary>Soft-deletes a farmer.</summary>
    [HttpDelete("farmers/{id:guid}")]
    public async Task<IActionResult> DeleteFarmer(Guid id, CancellationToken ct)
    {
        await farmers.DeleteAsync(id, ct);
        return NoContent();
    }

    // ── Baskets ───────────────────────────────────────────────────

    /// <summary>Gets all curated baskets.</summary>
    [HttpGet("baskets")]
    public async Task<ActionResult<IReadOnlyList<BasketDto>>> GetBaskets(CancellationToken ct)
        => Ok(await baskets.GetAsync(ct));

    /// <summary>Creates a basket.</summary>
    [HttpPost("baskets")]
    public async Task<ActionResult<BasketDto>> CreateBasket(BasketUpsertRequest request, CancellationToken ct)
        => Created(string.Empty, await baskets.CreateAsync(request, ct));

    /// <summary>Updates a basket.</summary>
    [HttpPut("baskets/{id:guid}")]
    public async Task<ActionResult<BasketDto>> UpdateBasket(Guid id, BasketUpsertRequest request, CancellationToken ct)
        => Ok(await baskets.UpdateAsync(id, request, ct));

    /// <summary>Soft-deletes a basket.</summary>
    [HttpDelete("baskets/{id:guid}")]
    public async Task<IActionResult> DeleteBasket(Guid id, CancellationToken ct)
    {
        await baskets.DeleteAsync(id, ct);
        return NoContent();
    }
}
