using AwakeningLifeBackend.Core.Domain.Entities;

namespace AwakeningLifeBackend.Core.Domain.Repositories;

public interface ISubscriptionRoleRepository
{
    Task<IEnumerable<SubscriptionRole>> GetSubscriptionRolesAsync(bool trackChanges, CancellationToken ct = default);
    Task<IEnumerable<SubscriptionRole>> GetSubscriptionRolesForProductAsync(string productId, bool trackChanges, CancellationToken ct = default);
    Task<SubscriptionRole?> GetSubscriptionRoleAsync(string productId, Guid roleId, bool trackChanges, CancellationToken ct = default);
    void CreateSubscriptionRoleForProduct(string productId, SubscriptionRole subscriptionRole);
    Task DeleteSubscriptionRoleAsync(SubscriptionRole subscriptionRole, CancellationToken ct = default);
}
