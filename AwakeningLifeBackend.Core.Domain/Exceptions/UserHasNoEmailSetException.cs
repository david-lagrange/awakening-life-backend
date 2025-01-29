namespace AwakeningLifeBackend.Core.Domain.Exceptions;

public sealed class UserHasNoEmailSetException : BadRequestException
{
    public UserHasNoEmailSetException(Guid userId)
        : base($"User with the id {userId} has no email set.")
    {
    }
}