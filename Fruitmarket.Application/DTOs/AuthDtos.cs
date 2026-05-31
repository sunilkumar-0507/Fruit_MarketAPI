namespace Fruitmarket.Application.DTOs;

public sealed record RegisterRequest(string FullName, string Email, string Password, string? PhoneNumber);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record ForgotPasswordRequest(string Email);
public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);
public sealed record VerifyEmailRequest(string Email, string Token);
public sealed record OtpLoginRequest(string PhoneNumber, string OtpCode);
public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc, UserDto User);
public sealed record UserDto(Guid Id, string FullName, string Email, string? PhoneNumber, bool EmailConfirmed, IReadOnlyList<string> Roles);
