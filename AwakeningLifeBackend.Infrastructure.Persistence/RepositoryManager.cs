using AwakeningLifeBackend.Core.Domain.Repositories;
using AwakeningLifeBackend.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace AwakeningLifeBackend.Infrastructure.Persistence;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IBaseEntityRepository> _baseEntityRepository;
    private readonly Lazy<IDependantEntityRepository> _dependantEntityRepository;
    private readonly Lazy<ISubscriptionFeatureRepository> _subscriptionFeatureRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _baseEntityRepository = new Lazy<IBaseEntityRepository>(() => new BaseEntityRepository(repositoryContext));
        _dependantEntityRepository = new Lazy<IDependantEntityRepository>(() => new DependantEntityRepository(repositoryContext));
        _subscriptionFeatureRepository = new Lazy<ISubscriptionFeatureRepository>(() => new SubscriptionFeatureRepository(repositoryContext));
    }

    public IBaseEntityRepository BaseEntity => _baseEntityRepository.Value;
    public IDependantEntityRepository DependantEntity => _dependantEntityRepository.Value;
    public ISubscriptionFeatureRepository SubscriptionFeature => _subscriptionFeatureRepository.Value;
    public Task SaveAsync(CancellationToken ct = default) => _repositoryContext.SaveChangesAsync(ct);
    public IDbContextTransaction BeginTransaction() => _repositoryContext.Database.BeginTransaction();
}
