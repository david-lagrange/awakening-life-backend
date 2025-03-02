using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AwakeningLifeBackend.Core.Services.Abstractions.Services;

public interface IWaitlistService
{
    Task<IEnumerable<WaitlistDto>> GetAllWaitlistEntriesAsync(bool trackChanges, CancellationToken ct = default);
    Task<WaitlistDto> GetWaitlistEntryByIdAsync(Guid waitlistId, bool trackChanges, CancellationToken ct = default);
    Task<WaitlistDto> CreateWaitlistEntryAsync(WaitlistForCreationDto waitlistDto, CancellationToken ct = default);
    Task DeleteWaitlistEntryAsync(Guid waitlistId, CancellationToken ct = default);
} 