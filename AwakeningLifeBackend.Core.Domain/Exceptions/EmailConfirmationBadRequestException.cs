namespace AwakeningLifeBackend.Core.Domain.Exceptions;

public sealed class EmailConfirmationBadRequestException : BadRequestException
{
    public EmailConfirmationBadRequestException(string message)
        : base($"Email confirmation failed: {message}")
    {
    }
} 