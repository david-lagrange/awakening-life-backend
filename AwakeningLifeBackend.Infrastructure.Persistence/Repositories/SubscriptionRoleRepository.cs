using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Core.Domain.Repositories;
using AwakeningLifeBackend.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionRoleRepository : RepositoryBase<SubscriptionRole>, ISubscriptionRoleRepository
{
    public SubscriptionRoleRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }
    public async Task<IEnumerable<SubscriptionRole>> GetSubscriptionRolesAsync(bool trackChanges, CancellationToken ct = default)
    {
        var subscriptionRoles = await FindAll(trackChanges)
            .Include(sr => sr.Role)
            .ToListAsync();

        return subscriptionRoles;
    }
    public async Task<IEnumerable<SubscriptionRole>> GetSubscriptionRolesForProductAsync(string productId, bool trackChanges, CancellationToken ct = default)
    {
        var subscriptionRoles = await FindByCondition(e => e.ProductId != null && e.ProductId.Equals(productId), trackChanges)
            .Include(sr => sr.Role)
            .ToListAsync();

        return subscriptionRoles;
    }

    public async Task<SubscriptionRole?> GetSubscriptionRoleAsync(string productId, Guid roleId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(e => e.ProductId != null && e.ProductId.Equals(productId) && e.RoleId.Equals(roleId), trackChanges)
        .Include(sr => sr.Role)
        .SingleOrDefaultAsync(ct);

    public void CreateSubscriptionRoleForProduct(string productId, SubscriptionRole subscriptionRole)
    {
        subscriptionRole.ProductId = productId;
        Create(subscriptionRole);
    }

    public async Task DeleteSubscriptionRoleAsync(SubscriptionRole subscriptionRole, CancellationToken ct = default)
    {

        Delete(subscriptionRole);

        await RepositoryContext.SaveChangesAsync(ct);
    }
}
