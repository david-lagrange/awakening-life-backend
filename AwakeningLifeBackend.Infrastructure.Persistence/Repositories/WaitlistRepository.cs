using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Repositories;

internal sealed class WaitlistRepository : RepositoryBase<Waitlist>, IWaitlistRepository
{
    public WaitlistRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Waitlist>> GetAllWaitlistEntriesAsync(bool trackChanges, CancellationToken ct = default) =>
        await FindAll(trackChanges)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(ct);

    public async Task<Waitlist?> GetWaitlistEntryByIdAsync(Guid waitlistId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(w => w.WaitlistId.Equals(waitlistId), trackChanges)
            .SingleOrDefaultAsync(ct);

    public async Task<Waitlist?> GetWaitlistEntryByEmailAsync(string email, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(w => w.Email.Equals(email), trackChanges)
            .SingleOrDefaultAsync(ct);

    public void CreateWaitlistEntry(Waitlist waitlist) => Create(waitlist);

    public void DeleteWaitlistEntry(Waitlist waitlist) => Delete(waitlist);
} 