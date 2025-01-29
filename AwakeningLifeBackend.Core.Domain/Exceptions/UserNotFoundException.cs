namespace AwakeningLifeBackend.Core.Domain.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid userId)
        : base($"User with id: {userId} doesn't exist in the database.")
    {
    }
    public UserNotFoundException(string customMessage) : base(customMessage) { }
    public static Exception CreateWithMessage(string custMessage)
    {
        return new UserNotFoundException(custMessage);
    }
}
