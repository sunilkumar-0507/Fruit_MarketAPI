using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class Address : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = "India";
    public bool IsDefault { get; set; }
}
