using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class ProductImage : BaseEntity
{
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
}
