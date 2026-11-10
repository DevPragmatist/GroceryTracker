using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace GroceryTracker.Infrastructure.Authentication;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Host) || _options.Port <= 0 || string.IsNullOrWhiteSpace(_options.FromEmail))
        {
            _logger.LogWarning(
                "SMTP is not configured. Password reset token for {Email}: {ResetToken}",
                email,
                resetToken);
            return;
        }

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            UseDefaultCredentials = string.IsNullOrWhiteSpace(_options.Username)
        };

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            client.Credentials = new NetworkCredential(_options.Username, _options.Password);
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = "GroceryTracker password reset",
            Body = $"Use the following reset token: {resetToken}",
            IsBodyHtml = false
        };

        message.To.Add(email);
        await client.SendMailAsync(message, cancellationToken);
    }
}
