using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = [];
}
