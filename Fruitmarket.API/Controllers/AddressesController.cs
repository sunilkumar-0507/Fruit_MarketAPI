using Fruitmarket.Application.Abstractions;
using Fruitmarket.Application.Common;
using Fruitmarket.Application.DTOs;
using Fruitmarket.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fruitmarket.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class AddressesController(IUnitOfWork uow, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AddressDto>>> Get(CancellationToken ct)
    {
        var items = await uow.Addresses.Query().Where(x => x.UserId == currentUser.UserId).Select(x => new AddressDto(x.Id, x.Line1, x.Line2, x.City, x.State, x.PostalCode, x.Country, x.IsDefault)).ToListAsync(ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> Add(AddressRequest request, CancellationToken ct)
    {
        if (request.IsDefault) await ClearDefaultAsync(ct);
        var address = new Address { UserId = currentUser.UserId, Line1 = request.Line1, Line2 = request.Line2, City = request.City, State = request.State, PostalCode = request.PostalCode, Country = request.Country, IsDefault = request.IsDefault };
        await uow.Addresses.AddAsync(address, ct);
        await uow.SaveChangesAsync(ct);
        return Created(string.Empty, ToDto(address));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AddressDto>> Update(Guid id, AddressRequest request, CancellationToken ct)
    {
        var address = await uow.Addresses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUser.UserId, ct) ?? throw new ApiException("Address not found", 404);
        if (request.IsDefault) await ClearDefaultAsync(ct);
        address.Line1 = request.Line1;
        address.Line2 = request.Line2;
        address.City = request.City;
        address.State = request.State;
        address.PostalCode = request.PostalCode;
        address.Country = request.Country;
        address.IsDefault = request.IsDefault;
        await uow.SaveChangesAsync(ct);
        return Ok(ToDto(address));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var address = await uow.Addresses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUser.UserId, ct) ?? throw new ApiException("Address not found", 404);
        uow.Addresses.Remove(address);
        await uow.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/default")]
    public async Task<IActionResult> SetDefault(Guid id, CancellationToken ct)
    {
        var address = await uow.Addresses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUser.UserId, ct) ?? throw new ApiException("Address not found", 404);
        await ClearDefaultAsync(ct);
        address.IsDefault = true;
        await uow.SaveChangesAsync(ct);
        return NoContent();
    }

    private async Task ClearDefaultAsync(CancellationToken ct)
    {
        var addresses = await uow.Addresses.Query().Where(x => x.UserId == currentUser.UserId && x.IsDefault).ToListAsync(ct);
        foreach (var address in addresses) address.IsDefault = false;
    }

    private static AddressDto ToDto(Address x) => new(x.Id, x.Line1, x.Line2, x.City, x.State, x.PostalCode, x.Country, x.IsDefault);
}
