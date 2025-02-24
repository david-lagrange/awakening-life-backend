using AwakeningLifeBackend.Core.Domain.Entities;

namespace AwakeningLifeBackend.Core.Domain.Repositories;

public interface ISubscriptionCancelationRepository
{
    Task<IEnumerable<SubscriptionCancelation>> GetSubscriptionCancelationsAsync(bool trackChanges, CancellationToken ct = default);
    Task<SubscriptionCancelation?> GetSubscriptionCancelationByIdAsync(Guid id, bool trackChanges, CancellationToken ct = default);
    Task<SubscriptionCancelation?> GetSubscriptionCancelationBySubscriptionIdAsync(string subscriptionId, bool trackChanges, CancellationToken ct = default);
    void CreateSubscriptionCancelation(SubscriptionCancelation subscriptionCancelation);
    Task DeleteSubscriptionCancelationAsync(SubscriptionCancelation subscriptionCancelation, CancellationToken ct = default);
} 