using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class Farmer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Village { get; set; }
    public string? Produce { get; set; }
    public int? WeeklySupplyKg { get; set; }
    public double? Rating { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
}
