namespace GroceryTracker.Infrastructure.Authentication;

public interface IEmailSender
{
    Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default);
}
