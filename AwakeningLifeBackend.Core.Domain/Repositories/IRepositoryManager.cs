﻿using Microsoft.EntityFrameworkCore.Storage;

namespace AwakeningLifeBackend.Core.Domain.Repositories;

public interface IRepositoryManager
{
    IBaseEntityRepository BaseEntity { get; }
    IDependantEntityRepository DependantEntity { get; }
    ISubscriptionFeatureRepository SubscriptionFeature { get; }
    ISubscriptionRoleRepository SubscriptionRole { get; }
    ISubscriptionCancelationRepository SubscriptionCancelation { get; }
    IWaitlistRepository Waitlist { get; }
    Task SaveAsync(CancellationToken ct = default);
    IDbContextTransaction BeginTransaction();
}
