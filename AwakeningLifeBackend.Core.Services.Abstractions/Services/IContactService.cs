using Shared.DataTransferObjects;

namespace AwakeningLifeBackend.Core.Services.Abstractions.Services;

public interface IContactService
{
    Task<bool> SendContactMessageAsync(ContactDto contactDto, CancellationToken ct = default);
} 