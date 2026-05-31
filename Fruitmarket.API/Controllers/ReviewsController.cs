using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class ReviewsController(IReviewService reviews) : ControllerBase
{
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ReviewDto>> Update(Guid id, ReviewRequest request, CancellationToken ct) => Ok(await reviews.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await reviews.DeleteAsync(id, ct);
        return NoContent();
    }
}
