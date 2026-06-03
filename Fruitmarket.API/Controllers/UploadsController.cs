using Fruitmarket.Application.Common;
using Fruitmarket.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

/// <summary>Admin-only file uploads (product images from the device).</summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/uploads")]
public sealed class UploadsController(IWebHostEnvironment env) : ControllerBase
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private const long MaxBytes = 5 * 1024 * 1024; // 5 MB

    /// <summary>Uploads a product image file from the device and returns its public URL.</summary>
    [HttpPost("image")]
    [RequestSizeLimit(MaxBytes)]
    public async Task<ActionResult<ImageUploadResponse>> UploadImage(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) throw new ApiException("No file uploaded", 400);
        if (file.Length > MaxBytes) throw new ApiException("File exceeds the 5 MB limit", 400);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext)) throw new ApiException("Unsupported image type. Allowed: jpg, jpeg, png, webp, gif", 400);
        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) throw new ApiException("Uploaded file is not an image", 400);

        var uploadsDir = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "products");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsDir, fileName);
        await using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream, ct);
        }

        var url = $"{Request.Scheme}://{Request.Host}/uploads/products/{fileName}";
        return Created(url, new ImageUploadResponse(url));
    }
}
