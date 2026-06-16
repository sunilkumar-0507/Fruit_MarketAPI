namespace Fruitmarket.Application.DTOs;

public sealed record ProductDto(Guid Id, string NameEn, string NameTa, string Slug, string? DescriptionEn, string? DescriptionTa, string? AboutEn, string? AboutTa, string? UsageEn, string? UsageTa, string? BenefitsEn, string? BenefitsTa, decimal Price, decimal? OriginalPrice, int StockQuantity, bool IsOutOfStock, CategoryDto? Category, IReadOnlyList<ProductImageDto> Images, double Rating);
public sealed record ProductImageDto(Guid Id, string Url, string? AltText, bool IsPrimary);
public sealed record ProductUpsertRequest(string NameEn, string NameTa, string? DescriptionEn, string? DescriptionTa, string? AboutEn, string? AboutTa, string? UsageEn, string? UsageTa, string? BenefitsEn, string? BenefitsTa, decimal Price, int StockQuantity, Guid CategoryId, IReadOnlyList<ProductImageRequest>? Images);
// Applies a percentage discount: stores the pre-discount price in OriginalPrice and lowers Price. 0 removes any discount.
public sealed record ProductDiscountRequest(decimal Percentage);
public sealed record ProductImageRequest(string Url, string? AltText, bool IsPrimary);
public sealed record ProductQuery(string? Search, Guid? CategoryId, decimal? MinPrice, decimal? MaxPrice, string? SortBy, bool Desc, int PageNumber = 1, int PageSize = 10, bool IncludeInactive = false);
public sealed record CategoryDto(Guid Id, string NameEn, string NameTa, string Slug, string? DescriptionEn, string? DescriptionTa);
public sealed record ImageUploadResponse(string Url);
public sealed record CategoryUpsertRequest(string NameEn, string NameTa, string? DescriptionEn, string? DescriptionTa);
