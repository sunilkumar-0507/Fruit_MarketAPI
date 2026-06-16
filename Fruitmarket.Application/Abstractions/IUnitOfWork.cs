using Fruitmarket.Domain.Entities;

namespace Fruitmarket.Application.Abstractions;

public interface IUnitOfWork
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<Product> Products { get; }
    IRepository<ProductImage> ProductImages { get; }
    IRepository<Category> Categories { get; }
    IRepository<Cart> Carts { get; }
    IRepository<CartItem> CartItems { get; }
    IRepository<Order> Orders { get; }
    IRepository<Review> Reviews { get; }
    IRepository<WishlistItem> WishlistItems { get; }
    IRepository<Address> Addresses { get; }
    IRepository<Coupon> Coupons { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    IRepository<Farmer> Farmers { get; }
    IRepository<Basket> Baskets { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
