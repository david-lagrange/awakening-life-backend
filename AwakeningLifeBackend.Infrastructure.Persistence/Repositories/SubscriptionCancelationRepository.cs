using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionCancelationRepository : RepositoryBase<SubscriptionCancelation>, ISubscriptionCancelationRepository
{
    public SubscriptionCancelationRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<SubscriptionCancelation>> GetSubscriptionCancelationsAsync(bool trackChanges, CancellationToken ct = default) =>
        await FindAll(trackChanges).ToListAsync(ct);

    public async Task<SubscriptionCancelation?> GetSubscriptionCancelationByIdAsync(Guid id, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(sc => sc.SubscriptionCancelationId.Equals(id), trackChanges)
            .SingleOrDefaultAsync(ct);

    public async Task<SubscriptionCancelation?> GetSubscriptionCancelationBySubscriptionIdAsync(string subscriptionId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(sc => sc.SubscriptionId != null && sc.SubscriptionId.Equals(subscriptionId), trackChanges)
            .SingleOrDefaultAsync(ct);

    public void CreateSubscriptionCancelation(SubscriptionCancelation subscriptionCancelation) =>
        Create(subscriptionCancelation);

    public async Task DeleteSubscriptionCancelationAsync(SubscriptionCancelation subscriptionCancelation, CancellationToken ct = default)
    {
        Delete(subscriptionCancelation);
        await RepositoryContext.SaveChangesAsync(ct);
    }
} 