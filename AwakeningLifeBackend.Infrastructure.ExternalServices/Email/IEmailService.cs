namespace AwakeningLifeBackend.Infrastructure.ExternalServices;

public interface IEmailService
{
    Task SendEmailAsync(string recipient, string passwordResetLink);
    Task SendEmailConfirmationAsync(string recipient, string confirmationLink);
    Task SendSubscriptionCanceledEmailAsync(string recipient, string resubscribeLink);
    Task SendWaitlistConfirmationEmailAsync(string recipient);
}
