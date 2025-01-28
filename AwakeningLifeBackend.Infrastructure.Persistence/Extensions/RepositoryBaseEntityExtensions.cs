using AwakeningLifeBackend.Core.Domain.Entities;
using System.Linq.Dynamic.Core;
using AwakeningLifeBackend.Infrastructure.Persistence.Extensions.Utility;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Extensions;

public static class RepositoryBaseEntityExtensions
{
    public static IQueryable<BaseEntity> Search(this IQueryable<BaseEntity> baseEntities, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return baseEntities;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return baseEntities.Where(e => e.Name != null && e.Name.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase));
    }

    public static IQueryable<BaseEntity> Sort(this IQueryable<BaseEntity> baseEntities, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return baseEntities.OrderBy(e => e.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<BaseEntity>(orderByQueryString);

        if (string.IsNullOrWhiteSpace(orderQuery))
            return baseEntities.OrderBy(e => e.Name);

        return baseEntities.OrderBy(orderQuery);
    }
}
