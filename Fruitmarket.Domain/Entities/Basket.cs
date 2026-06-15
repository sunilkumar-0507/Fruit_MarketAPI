using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

/// <summary>A curated gift/festival fruit hamper shown on the storefront baskets page.</summary>
public sealed class Basket : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    /// <summary>Image URLs, persisted as a JSON array in a single column.</summary>
    public List<string> Images { get; set; } = [];

    /// <summary>Human-readable contents summary, e.g. "Mango × 4, Banana × 6".</summary>
    public string Items { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
