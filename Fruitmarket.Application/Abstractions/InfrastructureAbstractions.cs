using Fruitmarket.Application.DTOs;
using Fruitmarket.Domain.Entities;

namespace Fruitmarket.Application.Abstractions;

public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}

public interface IPasswordHasher
{
    string Hash(string value);
    bool Verify(string value, string hash);
}

public interface IJwtTokenService
{
    AuthResponse CreateAuthResponse(User user);
    string HashToken(string token);
}

public interface ISlugService
{
    string GenerateSlug(string value);
}

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
}

public interface ISmsSender
{
    /// <summary>Sends a one-time password to an Indian 10-digit mobile number.</summary>
    Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct = default);
}
