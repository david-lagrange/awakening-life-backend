namespace AwakeningLifeBackend.Core.Domain.Exceptions;

public sealed class ProtectedUserException : BadRequestException
{
    public ProtectedUserException(Guid userId)
        : base($"The user with the id {userId} is protected.")
    {
    }
}