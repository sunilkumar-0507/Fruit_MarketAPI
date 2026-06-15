using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fruitmarket.Application.Abstractions;
using Fruitmarket.Application.Common;
using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Fruitmarket.Domain.Entities;
using Fruitmarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fruitmarket.Infrastructure.Services;

public sealed class AuthService(IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenService jwt, ICurrentUserService currentUser, IOptions<JwtOptions> jwtOptions, IEmailSender emailSender, IOptions<EmailOptions> emailOptions) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (await uow.Users.Query().AnyAsync(x => x.Email == request.Email, ct)) throw new ApiException("Email already registered", 409);
        var role = await uow.Roles.FirstOrDefaultAsync(x => x.Name == "Customer", ct) ?? new Role { Name = "Customer" };
        var user = new User { FullName = request.FullName, Email = request.Email.ToLowerInvariant(), PhoneNumber = request.PhoneNumber, PasswordHash = hasher.Hash(request.Password), EmailVerificationToken = Guid.NewGuid().ToString("N"), Roles = [role] };
        await uow.Users.AddAsync(user, ct);
        await uow.Carts.AddAsync(new Cart { User = user }, ct);
        var response = jwt.CreateAuthResponse(user);
        await uow.RefreshTokens.AddAsync(new RefreshToken { User = user, TokenHash = jwt.HashToken(response.RefreshToken), ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenDays) }, ct);
        await uow.SaveChangesAsync(ct);
        return response;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await uow.Users.Query().Include(x => x.Roles).FirstOrDefaultAsync(x => x.Email == request.Email.ToLowerInvariant(), ct);
        if (user is null || !hasher.Verify(request.Password, user.PasswordHash)) throw new ApiException("Invalid credentials", 401);
        return await CreateAndStoreTokens(user, ct);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        var hash = jwt.HashToken(request.RefreshToken);
        var token = await uow.RefreshTokens.Query().Include(x => x.User!).ThenInclude(x => x.Roles).FirstOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (token is null || !token.IsActive || token.User is null) throw new ApiException("Invalid refresh token", 401);
        token.RevokedAtUtc = DateTime.UtcNow;
        return await CreateAndStoreTokens(token.User, ct);
    }

    public async Task LogoutAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        var hash = jwt.HashToken(request.RefreshToken);
        var token = await uow.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (token is not null) token.RevokedAtUtc = DateTime.UtcNow;
        await uow.SaveChangesAsync(ct);
    }

    public async Task<UserDto> GetProfileAsync(CancellationToken ct)
    {
        var user = await uow.Users.Query().Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == currentUser.UserId, ct) ?? throw new ApiException("User not found", 404);
        return new UserDto(user.Id, user.FullName, user.Email, user.PhoneNumber, user.EmailConfirmed, user.Roles.Select(x => x.Name).ToArray());
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var page = Math.Max(1, pageNumber);
        var size = Math.Clamp(pageSize, 1, 100);
        var query = uow.Users.Query().Include(x => x.Roles).OrderByDescending(x => x.CreatedAtUtc);
        var total = await query.CountAsync(ct);
        var users = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var items = users
            .Select(u => new UserDto(u.Id, u.FullName, u.Email, u.PhoneNumber, u.EmailConfirmed, u.Roles.Select(r => r.Name).ToArray()))
            .ToList();
        return new PagedResult<UserDto>(items, page, size, total);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct)
    {
        var user = await uow.Users.FirstOrDefaultAsync(x => x.Email == request.Email.ToLowerInvariant(), ct);
        // Always return without revealing whether the email exists (prevents account enumeration).
        if (user is null) return;

        user.PasswordResetToken = Guid.NewGuid().ToString("N");
        user.PasswordResetTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(30);
        await uow.SaveChangesAsync(ct);

        var baseUrl = emailOptions.Value.FrontendUrl.TrimEnd('/');
        var resetUrl = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(user.Email)}&token={user.PasswordResetToken}";
        // Best-effort: a mail outage must not 500 the request or leak that the account exists.
        try
        {
            await emailSender.SendAsync(user.Email, "Reset your Tenkasi Fresh password", EmailTemplates.PasswordReset(resetUrl), ct);
        }
        catch
        {
            // swallowed intentionally — token is stored; user can retry
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var user = await uow.Users.FirstOrDefaultAsync(x => x.Email == request.Email.ToLowerInvariant() && x.PasswordResetToken == request.Token, ct) ?? throw new ApiException("Invalid reset token", 400);
        if (user.PasswordResetTokenExpiresAtUtc < DateTime.UtcNow) throw new ApiException("Reset token expired", 400);
        user.PasswordHash = hasher.Hash(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAtUtc = null;
        await uow.SaveChangesAsync(ct);
    }

    public async Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken ct)
    {
        var user = await uow.Users.FirstOrDefaultAsync(x => x.Email == request.Email.ToLowerInvariant() && x.EmailVerificationToken == request.Token, ct) ?? throw new ApiException("Invalid verification token", 400);
        user.EmailConfirmed = true;
        user.EmailVerificationToken = null;
        await uow.SaveChangesAsync(ct);
    }

    public async Task<AuthResponse> OtpLoginAsync(OtpLoginRequest request, CancellationToken ct)
    {
        var user = await uow.Users.Query().Include(x => x.Roles).FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber, ct) ?? throw new ApiException("Invalid OTP", 401);
        if (user.OtpCodeHash is not null && !hasher.Verify(request.OtpCode, user.OtpCodeHash)) throw new ApiException("Invalid OTP", 401);
        if (user.OtpExpiresAtUtc is not null && user.OtpExpiresAtUtc < DateTime.UtcNow) throw new ApiException("OTP expired", 401);
        return await CreateAndStoreTokens(user, ct);
    }

    private async Task<AuthResponse> CreateAndStoreTokens(User user, CancellationToken ct)
    {
        var response = jwt.CreateAuthResponse(user);
        await uow.RefreshTokens.AddAsync(new RefreshToken { UserId = user.Id, TokenHash = jwt.HashToken(response.RefreshToken), ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenDays) }, ct);
        await uow.SaveChangesAsync(ct);
        return response;
    }
}

public sealed class ProductService(IUnitOfWork uow, IMapper mapper, ISlugService slugger) : IProductService
{
    public async Task<PagedResult<ProductDto>> GetAsync(ProductQuery query, CancellationToken ct)
    {
        var page = Math.Max(1, query.PageNumber);
        var size = Math.Clamp(query.PageSize, 1, 100);
        var products = uow.Products.Query().Include(x => x.Category).Include(x => x.Images).Include(x => x.Reviews).AsNoTracking();
        if (!query.IncludeInactive) products = products.Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(query.Search)) products = products.Where(x => x.NameEn.Contains(query.Search) || x.NameTa.Contains(query.Search) || x.Slug.Contains(query.Search));
        if (query.CategoryId.HasValue) products = products.Where(x => x.CategoryId == query.CategoryId);
        if (query.MinPrice.HasValue) products = products.Where(x => x.Price >= query.MinPrice);
        if (query.MaxPrice.HasValue) products = products.Where(x => x.Price <= query.MaxPrice);
        products = (query.SortBy?.ToLowerInvariant(), query.Desc) switch
        {
            ("price", true) => products.OrderByDescending(x => x.Price),
            ("price", false) => products.OrderBy(x => x.Price),
            ("name", true) => products.OrderByDescending(x => x.NameEn),
            ("stock", true) => products.OrderByDescending(x => x.StockQuantity),
            ("stock", false) => products.OrderBy(x => x.StockQuantity),
            _ => products.OrderBy(x => x.NameEn)
        };
        var total = await products.CountAsync(ct);
        var items = await products.Skip((page - 1) * size).Take(size).ProjectTo<ProductDto>(mapper.ConfigurationProvider).ToListAsync(ct);
        return new PagedResult<ProductDto>(items, page, size, total);
    }

    public async Task<ProductDto> GetBySlugAsync(string slug, CancellationToken ct)
    {
        var product = await uow.Products.Query().Include(x => x.Category).Include(x => x.Images).Include(x => x.Reviews).AsNoTracking().FirstOrDefaultAsync(x => x.Slug == slug, ct) ?? throw new ApiException("Product not found", 404);
        return mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(ProductUpsertRequest request, CancellationToken ct)
    {
        var product = new Product { NameEn = request.NameEn, NameTa = request.NameTa, DescriptionEn = request.DescriptionEn, DescriptionTa = request.DescriptionTa, AboutEn = request.AboutEn, AboutTa = request.AboutTa, UsageEn = request.UsageEn, UsageTa = request.UsageTa, BenefitsEn = request.BenefitsEn, BenefitsTa = request.BenefitsTa, Price = request.Price, StockQuantity = request.StockQuantity, CategoryId = request.CategoryId, Slug = await GenerateUniqueSlugAsync(request.NameEn, null, ct) };
        foreach (var image in request.Images ?? []) product.Images.Add(new ProductImage { Url = image.Url, AltText = image.AltText, IsPrimary = image.IsPrimary });
        await uow.Products.AddAsync(product, ct);
        await uow.SaveChangesAsync(ct);
        return await GetBySlugAsync(product.Slug, ct);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, ProductUpsertRequest request, CancellationToken ct)
    {
        var product = await uow.Products.Query().Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new ApiException("Product not found", 404);
        var nameChanged = !string.Equals(product.NameEn, request.NameEn, StringComparison.Ordinal);
        product.NameEn = request.NameEn; product.NameTa = request.NameTa; product.DescriptionEn = request.DescriptionEn; product.DescriptionTa = request.DescriptionTa; product.AboutEn = request.AboutEn; product.AboutTa = request.AboutTa; product.UsageEn = request.UsageEn; product.UsageTa = request.UsageTa; product.BenefitsEn = request.BenefitsEn; product.BenefitsTa = request.BenefitsTa; product.Price = request.Price; product.StockQuantity = request.StockQuantity; product.CategoryId = request.CategoryId;
        if (nameChanged) product.Slug = await GenerateUniqueSlugAsync(request.NameEn, product.Id, ct);
        product.Images.Clear();
        foreach (var image in request.Images ?? []) product.Images.Add(new ProductImage { Url = image.Url, AltText = image.AltText, IsPrimary = image.IsPrimary });
        await uow.SaveChangesAsync(ct);
        return await GetBySlugAsync(product.Slug, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var product = await uow.Products.GetByIdAsync(id, ct) ?? throw new ApiException("Product not found", 404);
        product.IsDeleted = true;
        await uow.SaveChangesAsync(ct);
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, Guid? excludeId, CancellationToken ct)
    {
        var baseSlug = slugger.GenerateSlug(name);
        var slug = baseSlug;
        var suffix = 2;
        while (await uow.Products.Query().IgnoreQueryFilters().AnyAsync(x => x.Slug == slug && (excludeId == null || x.Id != excludeId.Value), ct))
            slug = $"{baseSlug}-{suffix++}";
        return slug;
    }
}

public sealed class CategoryService(IUnitOfWork uow, IMapper mapper, ISlugService slugger) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetAsync(CancellationToken ct) => await uow.Categories.Query().OrderBy(x => x.NameEn).ProjectTo<CategoryDto>(mapper.ConfigurationProvider).ToListAsync(ct);
    public async Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken ct) => mapper.Map<CategoryDto>(await uow.Categories.GetByIdAsync(id, ct) ?? throw new ApiException("Category not found", 404));
    public async Task<CategoryDto> CreateAsync(CategoryUpsertRequest request, CancellationToken ct) { var c = new Category { NameEn = request.NameEn, NameTa = request.NameTa, DescriptionEn = request.DescriptionEn, DescriptionTa = request.DescriptionTa, Slug = await GenerateUniqueSlugAsync(request.NameEn, null, ct) }; await uow.Categories.AddAsync(c, ct); await uow.SaveChangesAsync(ct); return mapper.Map<CategoryDto>(c); }
    public async Task<CategoryDto> UpdateAsync(Guid id, CategoryUpsertRequest request, CancellationToken ct) { var c = await uow.Categories.GetByIdAsync(id, ct) ?? throw new ApiException("Category not found", 404); var nameChanged = !string.Equals(c.NameEn, request.NameEn, StringComparison.Ordinal); c.NameEn = request.NameEn; c.NameTa = request.NameTa; c.DescriptionEn = request.DescriptionEn; c.DescriptionTa = request.DescriptionTa; if (nameChanged) c.Slug = await GenerateUniqueSlugAsync(request.NameEn, c.Id, ct); await uow.SaveChangesAsync(ct); return mapper.Map<CategoryDto>(c); }
    public async Task DeleteAsync(Guid id, CancellationToken ct) { var c = await uow.Categories.GetByIdAsync(id, ct) ?? throw new ApiException("Category not found", 404); uow.Categories.Remove(c); await uow.SaveChangesAsync(ct); }

    private async Task<string> GenerateUniqueSlugAsync(string name, Guid? excludeId, CancellationToken ct)
    {
        var baseSlug = slugger.GenerateSlug(name);
        var slug = baseSlug;
        var suffix = 2;
        while (await uow.Categories.Query().AnyAsync(x => x.Slug == slug && (excludeId == null || x.Id != excludeId.Value), ct))
            slug = $"{baseSlug}-{suffix++}";
        return slug;
    }
}

public sealed class CartService(IUnitOfWork uow, ICurrentUserService currentUser) : ICartService
{
    public async Task<CartDto> GetAsync(CancellationToken ct) => ToDto(await GetCart(ct));
    public async Task<CartDto> AddItemAsync(AddCartItemRequest request, CancellationToken ct)
    {
        var cart = await GetCart(ct);
        var product = await uow.Products.GetByIdAsync(request.ProductId, ct) ?? throw new ApiException("Product not found", 404);
        if (product.StockQuantity < request.Quantity) throw new ApiException("Insufficient stock", 409);
        var item = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);
        if (item is null) await uow.CartItems.AddAsync(new CartItem { CartId = cart.Id, ProductId = request.ProductId, Quantity = request.Quantity }, ct);
        else item.Quantity += request.Quantity;
        await uow.SaveChangesAsync(ct);
        return ToDto(await GetCart(ct));
    }
    public async Task<CartDto> UpdateItemAsync(Guid itemId, UpdateCartItemRequest request, CancellationToken ct) { var cart = await GetCart(ct); var item = cart.Items.FirstOrDefault(x => x.Id == itemId) ?? throw new ApiException("Cart item not found", 404); item.Quantity = request.Quantity; await uow.SaveChangesAsync(ct); return ToDto(await GetCart(ct)); }
    public async Task RemoveItemAsync(Guid itemId, CancellationToken ct) { var cart = await GetCart(ct); var item = cart.Items.FirstOrDefault(x => x.Id == itemId) ?? throw new ApiException("Cart item not found", 404); uow.CartItems.Remove(item); await uow.SaveChangesAsync(ct); }
    public async Task ClearAsync(CancellationToken ct) { var cart = await GetCart(ct); foreach (var item in cart.Items.ToList()) uow.CartItems.Remove(item); await uow.SaveChangesAsync(ct); }
    private async Task<Cart> GetCart(CancellationToken ct) => await uow.Carts.Query().Include(x => x.Items).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.UserId == currentUser.UserId, ct) ?? throw new ApiException("Cart not found", 404);
    private static CartDto ToDto(Cart cart) => new(cart.Id, cart.Items.Select(x => new CartItemDto(x.Id, x.ProductId, x.Product?.NameEn ?? "", x.Product?.Slug ?? "", x.Product?.Price ?? 0, x.Quantity, (x.Product?.Price ?? 0) * x.Quantity)).ToArray(), cart.Items.Sum(x => (x.Product?.Price ?? 0) * x.Quantity));
}

public sealed class OrderService(IUnitOfWork uow, ICurrentUserService currentUser) : IOrderService
{
    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct)
    {
        var cart = await uow.Carts.Query().Include(x => x.Items).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.UserId == currentUser.UserId, ct) ?? throw new ApiException("Cart not found", 404);
        if (!cart.Items.Any()) throw new ApiException("Cart is empty", 400);
        var subtotal = cart.Items.Sum(x => (x.Product?.Price ?? 0) * x.Quantity);
        var couponCode = request.CouponCode?.ToUpperInvariant();
        var coupon = string.IsNullOrWhiteSpace(couponCode) ? null : await uow.Coupons.FirstOrDefaultAsync(x => x.Code == couponCode && x.IsActive, ct);
        var discount = coupon is not null && subtotal >= (coupon.MinimumOrderAmount ?? 0) && coupon.StartsAtUtc <= DateTime.UtcNow && coupon.EndsAtUtc >= DateTime.UtcNow ? coupon.DiscountAmount : 0;
        var order = new Order { UserId = currentUser.UserId, OrderNumber = $"FM-{DateTime.UtcNow:yyyyMMddHHmmss}", ShippingAddressId = request.ShippingAddressId, CouponId = coupon?.Id, Subtotal = subtotal, Discount = discount, Total = Math.Max(0, subtotal - discount) };
        foreach (var item in cart.Items) { if (item.Product is null) continue; if (item.Product.StockQuantity < item.Quantity) throw new ApiException($"{item.Product.NameEn} is out of stock", 409); item.Product.StockQuantity -= item.Quantity; order.Items.Add(new OrderItem { ProductId = item.ProductId, ProductName = item.Product.NameEn, UnitPrice = item.Product.Price, Quantity = item.Quantity }); uow.CartItems.Remove(item); }
        await uow.Orders.AddAsync(order, ct);
        await uow.SaveChangesAsync(ct);
        return ToDto(order);
    }
    public async Task<IReadOnlyList<OrderDto>> GetHistoryAsync(CancellationToken ct)
    {
        var orders = await uow.Orders.Query().Include(x => x.Items).Include(x => x.User).Include(x => x.ShippingAddress).Where(x => x.UserId == currentUser.UserId).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);
        return orders.Select(ToDto).ToList();
    }
    public async Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct) => ToDto(await uow.Orders.Query().Include(x => x.Items).Include(x => x.User).Include(x => x.ShippingAddress).FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUser.UserId, ct) ?? throw new ApiException("Order not found", 404));
    public async Task<OrderDto> CancelAsync(Guid id, CancellationToken ct) { var order = await uow.Orders.Query().Include(x => x.Items).Include(x => x.User).Include(x => x.ShippingAddress).FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUser.UserId, ct) ?? throw new ApiException("Order not found", 404); if (order.Status is OrderStatus.Shipped or OrderStatus.Delivered) throw new ApiException("Order cannot be cancelled", 409); order.Status = OrderStatus.Cancelled; await uow.SaveChangesAsync(ct); return ToDto(order); }

    public async Task<PagedResult<OrderDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var page = Math.Max(1, pageNumber);
        var size = Math.Clamp(pageSize, 1, 100);
        var query = uow.Orders.Query().Include(x => x.Items).Include(x => x.User).Include(x => x.ShippingAddress).OrderByDescending(x => x.CreatedAtUtc);
        var total = await query.CountAsync(ct);
        var entities = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return new PagedResult<OrderDto>(entities.Select(ToDto).ToList(), page, size, total);
    }

    public async Task<OrderDto> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct)
    {
        var order = await uow.Orders.Query().Include(x => x.Items).Include(x => x.User).Include(x => x.ShippingAddress).FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new ApiException("Order not found", 404);
        order.Status = status;
        await uow.SaveChangesAsync(ct);
        return ToDto(order);
    }

    private static OrderDto ToDto(Order o) => new(
        o.Id, o.OrderNumber, o.Status, o.Subtotal, o.Discount, o.Total, o.TrackingNumber, o.CreatedAtUtc,
        o.User?.FullName, o.User?.Email, o.User?.PhoneNumber,
        o.ShippingAddress is null ? null : new AddressDto(o.ShippingAddress.Id, o.ShippingAddress.Line1, o.ShippingAddress.Line2, o.ShippingAddress.City, o.ShippingAddress.State, o.ShippingAddress.PostalCode, o.ShippingAddress.Country, o.ShippingAddress.IsDefault),
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToArray());
}

public sealed class ReviewService(IUnitOfWork uow, ICurrentUserService currentUser) : IReviewService
{
    public async Task<ReviewDto> AddAsync(string productSlug, ReviewRequest request, CancellationToken ct)
    {
        var product = await uow.Products.FirstOrDefaultAsync(x => x.Slug == productSlug, ct) ?? throw new ApiException("Product not found", 404);
        if (await uow.Reviews.Query().AnyAsync(x => x.ProductId == product.Id && x.UserId == currentUser.UserId, ct)) throw new ApiException("You have already reviewed this product", 409);
        var user = await uow.Users.GetByIdAsync(currentUser.UserId, ct);
        var review = new Review { ProductId = product.Id, UserId = currentUser.UserId, Rating = request.Rating, Comment = request.Comment };
        await uow.Reviews.AddAsync(review, ct);
        await uow.SaveChangesAsync(ct);
        return new(review.Id, review.ProductId, user?.FullName ?? "", review.Rating, review.Comment, review.CreatedAtUtc);
    }
    public async Task<IReadOnlyList<ReviewDto>> GetByProductAsync(string productSlug, CancellationToken ct) => await uow.Reviews.Query().Include(x => x.User).Include(x => x.Product).Where(x => x.Product!.Slug == productSlug).OrderByDescending(x => x.CreatedAtUtc).Select(x => new ReviewDto(x.Id, x.ProductId, x.User!.FullName, x.Rating, x.Comment, x.CreatedAtUtc)).ToListAsync(ct);
    public async Task<ReviewDto> UpdateAsync(Guid id, ReviewRequest request, CancellationToken ct) { var review = await uow.Reviews.GetByIdAsync(id, ct) ?? throw new ApiException("Review not found", 404); if (review.UserId != currentUser.UserId) throw new ApiException("Forbidden", 403); review.Rating = request.Rating; review.Comment = request.Comment; await uow.SaveChangesAsync(ct); var user = await uow.Users.GetByIdAsync(currentUser.UserId, ct); return new(review.Id, review.ProductId, user?.FullName ?? "", review.Rating, review.Comment, review.CreatedAtUtc); }
    public async Task DeleteAsync(Guid id, CancellationToken ct) { var review = await uow.Reviews.GetByIdAsync(id, ct) ?? throw new ApiException("Review not found", 404); if (review.UserId != currentUser.UserId) throw new ApiException("Forbidden", 403); uow.Reviews.Remove(review); await uow.SaveChangesAsync(ct); }
}

public sealed class WishlistService(IUnitOfWork uow, ICurrentUserService currentUser, IMapper mapper) : IWishlistService
{
    public async Task<IReadOnlyList<ProductDto>> GetAsync(CancellationToken ct) => await uow.WishlistItems.Query().Include(x => x.Product!).ThenInclude(x => x.Category).Include(x => x.Product!.Images).Include(x => x.Product!.Reviews).Where(x => x.UserId == currentUser.UserId).Select(x => x.Product!).ProjectTo<ProductDto>(mapper.ConfigurationProvider).ToListAsync(ct);
    public async Task AddAsync(Guid productId, CancellationToken ct) { if (!await uow.Products.Query().AnyAsync(x => x.Id == productId, ct)) throw new ApiException("Product not found", 404); if (!await uow.WishlistItems.Query().AnyAsync(x => x.UserId == currentUser.UserId && x.ProductId == productId, ct)) await uow.WishlistItems.AddAsync(new WishlistItem { UserId = currentUser.UserId, ProductId = productId }, ct); await uow.SaveChangesAsync(ct); }
    public async Task RemoveAsync(Guid productId, CancellationToken ct) { var item = await uow.WishlistItems.FirstOrDefaultAsync(x => x.UserId == currentUser.UserId && x.ProductId == productId, ct) ?? throw new ApiException("Wishlist item not found", 404); uow.WishlistItems.Remove(item); await uow.SaveChangesAsync(ct); }
}

public sealed class CouponService(IUnitOfWork uow) : ICouponService
{
    public async Task<IReadOnlyList<CouponDto>> GetAsync(CancellationToken ct) =>
        await uow.Coupons.Query().OrderBy(x => x.Code).Select(x => ToDto(x)).ToListAsync(ct);

    public async Task<CouponDto> GetByIdAsync(Guid id, CancellationToken ct) =>
        ToDto(await uow.Coupons.GetByIdAsync(id, ct) ?? throw new ApiException("Coupon not found", 404));

    public async Task<CouponDto> CreateAsync(CouponUpsertRequest request, CancellationToken ct)
    {
        if (await uow.Coupons.Query().AnyAsync(x => x.Code == request.Code, ct))
            throw new ApiException("Coupon code already exists", 409);
        var coupon = new Coupon { Code = request.Code.ToUpperInvariant(), DiscountAmount = request.DiscountAmount, MinimumOrderAmount = request.MinimumOrderAmount, StartsAtUtc = request.StartsAtUtc, EndsAtUtc = request.EndsAtUtc };
        await uow.Coupons.AddAsync(coupon, ct);
        await uow.SaveChangesAsync(ct);
        return ToDto(coupon);
    }

    public async Task<CouponDto> UpdateAsync(Guid id, CouponUpsertRequest request, CancellationToken ct)
    {
        var coupon = await uow.Coupons.GetByIdAsync(id, ct) ?? throw new ApiException("Coupon not found", 404);
        if (await uow.Coupons.Query().AnyAsync(x => x.Code == request.Code && x.Id != id, ct))
            throw new ApiException("Coupon code already exists", 409);
        coupon.Code = request.Code.ToUpperInvariant(); coupon.DiscountAmount = request.DiscountAmount; coupon.MinimumOrderAmount = request.MinimumOrderAmount; coupon.StartsAtUtc = request.StartsAtUtc; coupon.EndsAtUtc = request.EndsAtUtc;
        await uow.SaveChangesAsync(ct);
        return ToDto(coupon);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var coupon = await uow.Coupons.GetByIdAsync(id, ct) ?? throw new ApiException("Coupon not found", 404);
        uow.Coupons.Remove(coupon);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<CouponValidationResponse> ValidateAsync(string code, CancellationToken ct)
    {
        var coupon = await uow.Coupons.FirstOrDefaultAsync(x => x.Code == code.ToUpperInvariant() && x.IsActive, ct);
        if (coupon is null) return new(false, "Invalid coupon code", 0);
        if (coupon.StartsAtUtc > DateTime.UtcNow || coupon.EndsAtUtc < DateTime.UtcNow) return new(false, "Coupon has expired", 0);
        return new(true, null, coupon.DiscountAmount);
    }

    private static CouponDto ToDto(Coupon c) => new(c.Id, c.Code, c.DiscountAmount, c.MinimumOrderAmount, c.StartsAtUtc, c.EndsAtUtc, c.IsActive);
}

public sealed class FarmerService(IUnitOfWork uow, IMapper mapper) : IFarmerService
{
    public async Task<IReadOnlyList<FarmerDto>> GetAsync(CancellationToken ct)
    {
        var farmers = await uow.Farmers.Query().OrderBy(x => x.Name).ToListAsync(ct);
        return farmers.Select(mapper.Map<FarmerDto>).ToList();
    }

    public async Task<FarmerDto> GetByIdAsync(Guid id, CancellationToken ct) =>
        mapper.Map<FarmerDto>(await uow.Farmers.GetByIdAsync(id, ct) ?? throw new ApiException("Farmer not found", 404));

    public async Task<FarmerDto> CreateAsync(FarmerUpsertRequest request, CancellationToken ct)
    {
        var farmer = new Farmer { Name = request.Name, Village = request.Village, Produce = request.Produce, WeeklySupplyKg = request.WeeklySupplyKg, Rating = request.Rating, Phone = request.Phone, IsActive = request.IsActive };
        await uow.Farmers.AddAsync(farmer, ct);
        await uow.SaveChangesAsync(ct);
        return mapper.Map<FarmerDto>(farmer);
    }

    public async Task<FarmerDto> UpdateAsync(Guid id, FarmerUpsertRequest request, CancellationToken ct)
    {
        var farmer = await uow.Farmers.GetByIdAsync(id, ct) ?? throw new ApiException("Farmer not found", 404);
        farmer.Name = request.Name; farmer.Village = request.Village; farmer.Produce = request.Produce; farmer.WeeklySupplyKg = request.WeeklySupplyKg; farmer.Rating = request.Rating; farmer.Phone = request.Phone; farmer.IsActive = request.IsActive;
        await uow.SaveChangesAsync(ct);
        return mapper.Map<FarmerDto>(farmer);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var farmer = await uow.Farmers.GetByIdAsync(id, ct) ?? throw new ApiException("Farmer not found", 404);
        farmer.IsDeleted = true;
        await uow.SaveChangesAsync(ct);
    }
}

public sealed class BasketService(IUnitOfWork uow, IMapper mapper) : IBasketService
{
    public async Task<IReadOnlyList<BasketDto>> GetAsync(CancellationToken ct)
    {
        var baskets = await uow.Baskets.Query().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(ct);
        return baskets.Select(mapper.Map<BasketDto>).ToList();
    }

    public async Task<BasketDto> GetByIdAsync(Guid id, CancellationToken ct) =>
        mapper.Map<BasketDto>(await uow.Baskets.GetByIdAsync(id, ct) ?? throw new ApiException("Basket not found", 404));

    public async Task<BasketDto> CreateAsync(BasketUpsertRequest request, CancellationToken ct)
    {
        var basket = new Basket { Name = request.Name, Description = request.Description, Price = request.Price, Images = request.Images?.ToList() ?? [], Items = request.Items, IsActive = request.IsActive };
        await uow.Baskets.AddAsync(basket, ct);
        await uow.SaveChangesAsync(ct);
        return mapper.Map<BasketDto>(basket);
    }

    public async Task<BasketDto> UpdateAsync(Guid id, BasketUpsertRequest request, CancellationToken ct)
    {
        var basket = await uow.Baskets.GetByIdAsync(id, ct) ?? throw new ApiException("Basket not found", 404);
        basket.Name = request.Name; basket.Description = request.Description; basket.Price = request.Price; basket.Images = request.Images?.ToList() ?? []; basket.Items = request.Items; basket.IsActive = request.IsActive;
        await uow.SaveChangesAsync(ct);
        return mapper.Map<BasketDto>(basket);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var basket = await uow.Baskets.GetByIdAsync(id, ct) ?? throw new ApiException("Basket not found", 404);
        basket.IsDeleted = true;
        await uow.SaveChangesAsync(ct);
    }
}
