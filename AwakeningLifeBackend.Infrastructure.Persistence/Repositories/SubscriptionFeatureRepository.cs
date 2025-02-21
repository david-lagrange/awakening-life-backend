using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Core.Domain.Repositories;
using AwakeningLifeBackend.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionFeatureRepository : RepositoryBase<SubscriptionFeature>, ISubscriptionFeatureRepository
{
    public SubscriptionFeatureRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<SubscriptionFeature>> GetSubscriptionFeaturesAsync(string productId, bool trackChanges, CancellationToken ct = default)
    {
        var subscriptionFeatures = await FindByCondition(e => e.ProductId != null && e.ProductId.Equals(productId), trackChanges)
            .OrderBy(e => e.FeatureOrder).ToListAsync();

        return subscriptionFeatures;
    }

    public async Task<SubscriptionFeature?> GetSubscriptionFeatureAsync(string productId, Guid id, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(e => e.ProductId != null && e.ProductId.Equals(productId) && e.SubscriptionFeatureId.Equals(id), trackChanges)
        .SingleOrDefaultAsync(ct);

    public void CreateSubscriptionFeatureForProduct(string productId, SubscriptionFeature subscriptionFeature)
    {
        subscriptionFeature.ProductId = productId;
        Create(subscriptionFeature);
    }

    public async Task DeleteSubscriptionFeatureAsync(SubscriptionFeature subscriptionFeature, CancellationToken ct = default)
    {

        Delete(subscriptionFeature);

        await RepositoryContext.SaveChangesAsync(ct);
    }
}
