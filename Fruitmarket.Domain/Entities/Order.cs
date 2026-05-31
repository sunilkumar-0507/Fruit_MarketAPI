using Fruitmarket.Domain.Common;
using Fruitmarket.Domain.Enums;

namespace Fruitmarket.Domain.Entities;

public sealed class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? TrackingNumber { get; set; }
    public Guid? CouponId { get; set; }
    public Coupon? Coupon { get; set; }
    public Guid? ShippingAddressId { get; set; }
    public Address? ShippingAddress { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
}
