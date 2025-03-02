using AwakeningLifeBackend.Core.Domain.Entities;

namespace AwakeningLifeBackend.Core.Domain.Repositories;

public interface IWaitlistRepository
{
    Task<IEnumerable<Waitlist>> GetAllWaitlistEntriesAsync(bool trackChanges, CancellationToken ct = default);
    Task<Waitlist?> GetWaitlistEntryByIdAsync(Guid waitlistId, bool trackChanges, CancellationToken ct = default);
    Task<Waitlist?> GetWaitlistEntryByEmailAsync(string email, bool trackChanges, CancellationToken ct = default);
    void CreateWaitlistEntry(Waitlist waitlist);
    void DeleteWaitlistEntry(Waitlist waitlist);
}