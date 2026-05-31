using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class Category : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameTa { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionTa { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
