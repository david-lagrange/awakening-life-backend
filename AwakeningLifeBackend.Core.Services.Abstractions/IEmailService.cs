namespace AwakeningLifeBackend.Core.Services.Abstractions;

public interface IEmailService
{
    Task SendEmailAsync(string recipient, string passwordResetLink);
    Task SendEmailConfirmationAsync(string recipient, string confirmationLink);
}
