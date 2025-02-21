using AwakeningLifeBackend.Core.Domain.Entities;

namespace AwakeningLifeBackend.Core.Domain.Repositories;

public interface ISubscriptionFeatureRepository
{
    Task<IEnumerable<SubscriptionFeature>> GetSubscriptionFeaturesAsync(Guid productId, bool trackChanges, CancellationToken ct = default);
    Task<SubscriptionFeature?> GetSubscriptionFeatureAsync(Guid productId, Guid id, bool trackChanges, CancellationToken ct = default);
    void CreateSubscriptionFeatureForProduct(string productId, SubscriptionFeature subscriptionFeature);
    Task DeleteSubscriptionFeatureAsync(SubscriptionFeature subscriptionFeature, CancellationToken ct = default);
}
