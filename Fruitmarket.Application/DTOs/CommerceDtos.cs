using Fruitmarket.Domain.Enums;

namespace Fruitmarket.Application.DTOs;

public sealed record CartDto(Guid Id, IReadOnlyList<CartItemDto> Items, decimal Total);
public sealed record CartItemDto(Guid Id, Guid ProductId, string ProductName, string Slug, decimal UnitPrice, int Quantity, decimal LineTotal);
public sealed record AddCartItemRequest(Guid ProductId, int Quantity);
public sealed record UpdateCartItemRequest(int Quantity);
public sealed record CreateOrderRequest(Guid? ShippingAddressId, string? CouponCode, string? PaymentMethod = null);
public sealed record OrderDto(Guid Id, string OrderNumber, OrderStatus Status, decimal Subtotal, decimal Discount, decimal Total, string? TrackingNumber, DateTime CreatedAtUtc, string? CustomerName, string? CustomerEmail, string? CustomerPhone, AddressDto? ShippingAddress, IReadOnlyList<OrderItemDto> Items, string PaymentMethod, string PaymentStatus, string? PaymentTransactionId);
public sealed record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);

// ── Payment (Cashfree) ────────────────────────────────────────────────────────
/// <summary>Customer overrides passed from the storefront (the User record may lack a phone, which Cashfree requires).</summary>
public sealed record CreatePaymentSessionRequest(string? CustomerName, string? CustomerPhone);
public sealed record PaymentSessionDto(string OrderId, string PaymentSessionId, string CashfreeOrderId, decimal Amount);
public sealed record UpdateOrderStatusRequest(OrderStatus Status);
public sealed record ReviewDto(Guid Id, Guid ProductId, string UserName, int Rating, string? Comment, DateTime CreatedAtUtc);
public sealed record ReviewRequest(int Rating, string? Comment);
public sealed record AddressDto(Guid Id, string Line1, string? Line2, string City, string State, string PostalCode, string Country, bool IsDefault);
public sealed record AddressRequest(string Line1, string? Line2, string City, string State, string PostalCode, string Country, bool IsDefault);
public sealed record CouponDto(Guid Id, string Code, decimal DiscountAmount, decimal? MinimumOrderAmount, DateTime StartsAtUtc, DateTime EndsAtUtc, bool IsActive);
public sealed record CouponUpsertRequest(string Code, decimal DiscountAmount, decimal? MinimumOrderAmount, DateTime StartsAtUtc, DateTime EndsAtUtc);
public sealed record CouponValidationResponse(bool IsValid, string? Message, decimal DiscountAmount);
