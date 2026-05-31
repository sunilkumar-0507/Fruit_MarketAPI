using Fruitmarket.Application.DTOs;
using Fruitmarket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fruitmarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Registers a customer account.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct) => Created(string.Empty, await authService.RegisterAsync(request, ct));

    /// <summary>Authenticates using email and password.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct) => Ok(await authService.LoginAsync(request, ct));

    /// <summary>Rotates a refresh token.</summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request, CancellationToken ct) => Ok(await authService.RefreshAsync(request, ct));

    /// <summary>Revokes a refresh token.</summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request, CancellationToken ct)
    {
        await authService.LogoutAsync(request, ct);
        return NoContent();
    }

    /// <summary>Gets the current authenticated user profile.</summary>
    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> Profile(CancellationToken ct) => Ok(await authService.GetProfileAsync(ct));

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken ct)
    {
        await authService.ForgotPasswordAsync(request, ct);
        return Accepted();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken ct)
    {
        await authService.ResetPasswordAsync(request, ct);
        return NoContent();
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request, CancellationToken ct)
    {
        await authService.VerifyEmailAsync(request, ct);
        return NoContent();
    }

    [HttpPost("otp-login")]
    public async Task<ActionResult<AuthResponse>> OtpLogin(OtpLoginRequest request, CancellationToken ct) => Ok(await authService.OtpLoginAsync(request, ct));
}
