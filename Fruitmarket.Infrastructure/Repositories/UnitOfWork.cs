using Fruitmarket.Application.Abstractions;
using Fruitmarket.Domain.Entities;
using Fruitmarket.Infrastructure.Data;

namespace Fruitmarket.Infrastructure.Repositories;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public IRepository<User> Users { get; } = new Repository<User>(context);
    public IRepository<Role> Roles { get; } = new Repository<Role>(context);
    public IRepository<Product> Products { get; } = new Repository<Product>(context);
    public IRepository<ProductImage> ProductImages { get; } = new Repository<ProductImage>(context);
    public IRepository<Category> Categories { get; } = new Repository<Category>(context);
    public IRepository<Cart> Carts { get; } = new Repository<Cart>(context);
    public IRepository<CartItem> CartItems { get; } = new Repository<CartItem>(context);
    public IRepository<Order> Orders { get; } = new Repository<Order>(context);
    public IRepository<Review> Reviews { get; } = new Repository<Review>(context);
    public IRepository<WishlistItem> WishlistItems { get; } = new Repository<WishlistItem>(context);
    public IRepository<Address> Addresses { get; } = new Repository<Address>(context);
    public IRepository<Coupon> Coupons { get; } = new Repository<Coupon>(context);
    public IRepository<RefreshToken> RefreshTokens { get; } = new Repository<RefreshToken>(context);
    public IRepository<Farmer> Farmers { get; } = new Repository<Farmer>(context);
    public IRepository<Basket> Baskets { get; } = new Repository<Basket>(context);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}
