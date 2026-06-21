using Fruitmarket.Domain.Common;

namespace Fruitmarket.Domain.Entities;

public sealed class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    // Optional: registration now keys on PhoneNumber, so a user may have no email.
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string? EmailVerificationToken { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiresAtUtc { get; set; }
    public string? OtpCodeHash { get; set; }
    public DateTime? OtpExpiresAtUtc { get; set; }
    public ICollection<Role> Roles { get; set; } = [];
    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
    public ICollection<Address> Addresses { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
