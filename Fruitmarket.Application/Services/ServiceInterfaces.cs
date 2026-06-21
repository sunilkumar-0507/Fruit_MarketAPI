using Fruitmarket.Application.Common;
using Fruitmarket.Application.DTOs;
using Fruitmarket.Domain.Enums;

namespace Fruitmarket.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken ct);
    Task LogoutAsync(RefreshTokenRequest request, CancellationToken ct);
    Task<UserDto> GetProfileAsync(CancellationToken ct);
    Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize, CancellationToken ct);
    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct);
    Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken ct);
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct);
    Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken ct);
    Task<AuthResponse> OtpLoginAsync(OtpLoginRequest request, CancellationToken ct);
}

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAsync(ProductQuery query, CancellationToken ct);
    Task<ProductDto> GetBySlugAsync(string slug, CancellationToken ct);
    Task<ProductDto> CreateAsync(ProductUpsertRequest request, CancellationToken ct);
    Task<ProductDto> UpdateAsync(Guid id, ProductUpsertRequest request, CancellationToken ct);
    Task<ProductDto> ApplyDiscountAsync(Guid id, decimal percentage, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAsync(CancellationToken ct);
    Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<CategoryDto> CreateAsync(CategoryUpsertRequest request, CancellationToken ct);
    Task<CategoryDto> UpdateAsync(Guid id, CategoryUpsertRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public interface ICartService
{
    Task<CartDto> GetAsync(CancellationToken ct);
    Task<CartDto> AddItemAsync(AddCartItemRequest request, CancellationToken ct);
    Task<CartDto> UpdateItemAsync(Guid itemId, UpdateCartItemRequest request, CancellationToken ct);
    Task RemoveItemAsync(Guid itemId, CancellationToken ct);
    Task ClearAsync(CancellationToken ct);
}

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct);
    Task<IReadOnlyList<OrderDto>> GetHistoryAsync(CancellationToken ct);
    Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<OrderDto> CancelAsync(Guid id, CancellationToken ct);
    Task<PagedResult<OrderDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken ct);
    Task<OrderDto> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct);
}

public interface IPaymentService
{
    /// <summary>Creates a Cashfree order for an existing local order and returns the JS-SDK payment session.</summary>
    Task<PaymentSessionDto> CreateSessionAsync(Guid orderId, CreatePaymentSessionRequest request, CancellationToken ct);
    /// <summary>Queries Cashfree for the order result and persists payment status + transaction id. Returns the updated order.</summary>
    Task<OrderDto> VerifyAsync(Guid orderId, CancellationToken ct);
    /// <summary>Processes a Cashfree webhook (server-to-server confirmation). Signature is verified before any DB write.</summary>
    Task ProcessWebhookAsync(string rawBody, string signature, string timestamp, CancellationToken ct);
}

public interface IReviewService
{
    Task<ReviewDto> AddAsync(string productSlug, ReviewRequest request, CancellationToken ct);
    Task<IReadOnlyList<ReviewDto>> GetByProductAsync(string productSlug, CancellationToken ct);
    Task<ReviewDto> UpdateAsync(Guid id, ReviewRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public interface IWishlistService
{
    Task<IReadOnlyList<ProductDto>> GetAsync(CancellationToken ct);
    Task AddAsync(Guid productId, CancellationToken ct);
    Task RemoveAsync(Guid productId, CancellationToken ct);
}

public interface ICouponService
{
    Task<IReadOnlyList<CouponDto>> GetAsync(CancellationToken ct);
    Task<CouponDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<CouponDto> CreateAsync(CouponUpsertRequest request, CancellationToken ct);
    Task<CouponDto> UpdateAsync(Guid id, CouponUpsertRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<CouponValidationResponse> ValidateAsync(string code, CancellationToken ct);
}

public interface IFarmerService
{
    Task<IReadOnlyList<FarmerDto>> GetAsync(CancellationToken ct);
    Task<FarmerDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<FarmerDto> CreateAsync(FarmerUpsertRequest request, CancellationToken ct);
    Task<FarmerDto> UpdateAsync(Guid id, FarmerUpsertRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public interface IBasketService
{
    Task<IReadOnlyList<BasketDto>> GetAsync(CancellationToken ct);
    Task<BasketDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<BasketDto> CreateAsync(BasketUpsertRequest request, CancellationToken ct);
    Task<BasketDto> UpdateAsync(Guid id, BasketUpsertRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
