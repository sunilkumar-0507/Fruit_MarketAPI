using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    // Decimal to preserve the exact gram-based quantity ordered (0.25 = 250g).
    public decimal Quantity { get; set; }
}
