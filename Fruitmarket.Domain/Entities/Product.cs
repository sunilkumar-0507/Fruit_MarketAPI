using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class Product : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameTa { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionTa { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
