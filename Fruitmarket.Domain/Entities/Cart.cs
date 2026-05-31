using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class Cart : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public ICollection<CartItem> Items { get; set; } = [];
}
