﻿using Microsoft.EntityFrameworkCore.Storage;

namespace AwakeningLifeBackend.Core.Domain.Repositories;

public interface IRepositoryManager
{
    IBaseEntityRepository BaseEntity { get; }
    IDependantEntityRepository DependantEntity { get; }
    Task SaveAsync(CancellationToken ct = default);
    IDbContextTransaction BeginTransaction();
}
