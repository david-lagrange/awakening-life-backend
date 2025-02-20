using AutoMapper;
using AwakeningLifeBackend.Core.Domain.ConfigurationModels;
using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Core.Domain.Repositories;
using AwakeningLifeBackend.Core.Services.Abstractions;
using AwakeningLifeBackend.Infrastructure.ExternalServices;
using LoggingService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AwakeningLifeBackend.Core.Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IBaseEntityService> _baseEntityService;
    private readonly Lazy<IDependantEntityService> _dependantEntityService;
    private readonly Lazy<IAuthenticationService> _authenticationService;
    private readonly Lazy<IUserService> _userService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JwtConfiguration> configuration,
        IEmailService emailService)
    {
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, mapper, userManager, roleManager, configuration));

        _baseEntityService = new Lazy<IBaseEntityService>(() => new BaseEntityService(repositoryManager, logger, mapper));
        _dependantEntityService = new Lazy<IDependantEntityService>(() => new DependantEntityService(repositoryManager, logger, mapper));
        _userService = new Lazy<IUserService>(() => new UserService(logger, mapper, userManager, emailService));
    }

    public IAuthenticationService AuthenticationService => _authenticationService.Value;
    public IBaseEntityService BaseEntityService => _baseEntityService.Value;
    public IDependantEntityService DependantEntityService => _dependantEntityService.Value;
    public IUserService UserService => _userService.Value;

}
