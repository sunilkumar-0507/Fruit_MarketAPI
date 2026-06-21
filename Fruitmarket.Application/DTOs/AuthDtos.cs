namespace Fruitmarket.Application.DTOs;

public sealed record RegisterRequest(string FullName, string PhoneNumber, string Password, string? Email = null);
public sealed record LoginRequest(string PhoneNumber, string Password);
public sealed record RefreshTokenRequest(string RefreshToken);
// Phone-based password reset via OTP.
public sealed record ForgotPasswordRequest(string PhoneNumber);
public sealed record VerifyOtpRequest(string PhoneNumber, string Otp);
public sealed record VerifyOtpResponse(string Token);
public sealed record ResetPasswordRequest(string Token, string NewPassword);
public sealed record VerifyEmailRequest(string Email, string Token);
public sealed record OtpLoginRequest(string PhoneNumber, string OtpCode);
public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc, UserDto User);
public sealed record UserDto(Guid Id, string FullName, string? Email, string? PhoneNumber, bool EmailConfirmed, IReadOnlyList<string> Roles);
