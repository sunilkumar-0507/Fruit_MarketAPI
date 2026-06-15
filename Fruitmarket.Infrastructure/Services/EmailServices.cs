using Fruitmarket.Application.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Fruitmarket.Infrastructure.Services;

public sealed class EmailOptions
{
    /// <summary>When false (or SMTP host/credentials missing), emails are logged instead of sent — handy in dev.</summary>
    public bool Enabled { get; set; }
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public bool UseSsl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "Tenkasi Fresh";
    /// <summary>Base URL of the storefront, used to build links inside emails (e.g. the password-reset page).</summary>
    public string FrontendUrl { get; set; } = "http://localhost:3000";
}

public sealed class SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.SmtpHost))
        {
            logger.LogWarning("Email sending disabled — would have sent to {To} with subject \"{Subject}\".\n{Body}", toEmail, subject, htmlBody);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, string.IsNullOrWhiteSpace(_options.FromAddress) ? _options.Username : _options.FromAddress));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        var socketOptions = _options.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;
        await client.ConnectAsync(_options.SmtpHost, _options.SmtpPort, socketOptions, ct);
        if (!string.IsNullOrWhiteSpace(_options.Username))
            await client.AuthenticateAsync(_options.Username, _options.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
        logger.LogInformation("Email sent to {To} with subject \"{Subject}\".", toEmail, subject);
    }
}

/// <summary>Builds the HTML bodies used by the auth flows so the templates live in one place.</summary>
public static class EmailTemplates
{
    public static string PasswordReset(string resetUrl) => $"""
        <div style="font-family:Segoe UI,Arial,sans-serif;max-width:480px;margin:auto;color:#1f2937">
          <h2 style="color:#3d7a20">Reset your password</h2>
          <p>We received a request to reset your Tenkasi Fresh password. Click the button below to choose a new one. This link expires in 30 minutes.</p>
          <p style="text-align:center;margin:28px 0">
            <a href="{resetUrl}" style="background:#3d7a20;color:#fff;text-decoration:none;padding:12px 24px;border-radius:10px;font-weight:600">Reset Password</a>
          </p>
          <p style="font-size:13px;color:#6b7280">If the button doesn't work, copy this link into your browser:<br><a href="{resetUrl}">{resetUrl}</a></p>
          <p style="font-size:13px;color:#6b7280">If you didn't request this, you can safely ignore this email.</p>
        </div>
        """;
}
