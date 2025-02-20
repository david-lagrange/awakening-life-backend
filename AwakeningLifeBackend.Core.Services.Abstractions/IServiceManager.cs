using AwakeningLifeBackend.Core.Services.Abstractions.Services;

namespace AwakeningLifeBackend.Core.Services.Abstractions;

public interface IServiceManager
{
    IBaseEntityService BaseEntityService { get; }
    IDependantEntityService DependantEntityService { get; }
    IAuthenticationService AuthenticationService { get; }
    IUserService UserService { get; }
}
