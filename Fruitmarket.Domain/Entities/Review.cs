using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class Review : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
