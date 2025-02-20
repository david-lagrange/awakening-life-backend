namespace AwakeningLifeBackend.Core.Domain.Exceptions;

public sealed class UpdatePasswordBadRequest : BadRequestException
{
    public UpdatePasswordBadRequest(string message)
        : base(message)
    {

    }
}