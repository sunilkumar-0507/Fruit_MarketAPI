using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(IOrderService orders) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request, CancellationToken ct) => Created(string.Empty, await orders.CreateAsync(request, ct));

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> Get(CancellationToken ct) => Ok(await orders.GetHistoryAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct) => Ok(await orders.GetByIdAsync(id, ct));

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<OrderDto>> Cancel(Guid id, CancellationToken ct) => Ok(await orders.CancelAsync(id, ct));
}
