using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class Product : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameTa { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionTa { get; set; }
    public string? AboutEn { get; set; }
    public string? AboutTa { get; set; }
    public string? UsageEn { get; set; }
    public string? UsageTa { get; set; }
    public string? BenefitsEn { get; set; }
    public string? BenefitsTa { get; set; }
    public decimal Price { get; set; }
    // Pre-discount price. Null = no discount (sells at Price). Set by the discount endpoint;
    // when set, it holds the original price and Price holds the discounted price.
    public decimal? OriginalPrice { get; set; }
    public int StockQuantity { get; set; }
    // Manual admin override: mark a product out of stock independently of StockQuantity.
    public bool IsOutOfStock { get; set; }
    // Denormalised average of all reviews' ratings; recalculated whenever a review is added/updated/removed.
    public double Rating { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
