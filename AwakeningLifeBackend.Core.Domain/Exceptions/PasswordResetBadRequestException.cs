namespace AwakeningLifeBackend.Core.Domain.Exceptions;

public sealed class PasswordResetBadRequestException : BadRequestException
{
    public PasswordResetBadRequestException(string message)
        : base($"Password reset failed: {message}")
    {
    }
} 