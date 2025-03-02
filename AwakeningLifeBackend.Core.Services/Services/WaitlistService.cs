using AutoMapper;
using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Core.Domain.Exceptions;
using AwakeningLifeBackend.Core.Domain.Repositories;
using AwakeningLifeBackend.Core.Services.Abstractions.Services;
using AwakeningLifeBackend.Infrastructure.ExternalServices;
using LoggingService;
using Shared.DataTransferObjects;

namespace AwakeningLifeBackend.Core.Services.Services;

internal sealed class WaitlistService : IWaitlistService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public WaitlistService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IEmailService emailService)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<IEnumerable<WaitlistDto>> GetAllWaitlistEntriesAsync(bool trackChanges, CancellationToken ct = default)
    {
        var waitlistEntries = await _repository.Waitlist.GetAllWaitlistEntriesAsync(trackChanges, ct);
        var waitlistDtos = _mapper.Map<IEnumerable<WaitlistDto>>(waitlistEntries);
        return waitlistDtos;
    }

    public async Task<WaitlistDto> GetWaitlistEntryByIdAsync(Guid waitlistId, bool trackChanges, CancellationToken ct = default)
    {
        var waitlistEntry = await _repository.Waitlist.GetWaitlistEntryByIdAsync(waitlistId, trackChanges, ct);
        
        if (waitlistEntry == null)
            throw new WaitlistNotFoundException(waitlistId);
            
        var waitlistDto = _mapper.Map<WaitlistDto>(waitlistEntry);
        return waitlistDto;
    }

    public async Task<WaitlistDto> CreateWaitlistEntryAsync(WaitlistForCreationDto waitlistDto, CancellationToken ct = default)
    {
        // Check if email already exists in waitlist
        var existingEntry = await _repository.Waitlist.GetWaitlistEntryByEmailAsync(waitlistDto.Email, false, ct);
        if (existingEntry != null)
        {
            _logger.LogWarning($"Email {waitlistDto.Email} is already on the waitlist.");
            return _mapper.Map<WaitlistDto>(existingEntry); // Return existing entry instead of creating duplicate
        }
        
        var waitlistEntry = _mapper.Map<Waitlist>(waitlistDto);
        waitlistEntry.WaitlistId = Guid.NewGuid();
        waitlistEntry.CreatedAt = DateTime.UtcNow;
        
        _repository.Waitlist.CreateWaitlistEntry(waitlistEntry);
        await _repository.SaveAsync(ct);
        
        // Send confirmation email to the user
        await _emailService.SendWaitlistConfirmationEmailAsync(waitlistDto.Email);
        
        return _mapper.Map<WaitlistDto>(waitlistEntry);
    }

    public async Task DeleteWaitlistEntryAsync(Guid waitlistId, CancellationToken ct = default)
    {
        var waitlistEntry = await _repository.Waitlist.GetWaitlistEntryByIdAsync(waitlistId, true, ct);
        
        if (waitlistEntry == null)
            throw new WaitlistNotFoundException(waitlistId);
            
        _repository.Waitlist.DeleteWaitlistEntry(waitlistEntry);
        await _repository.SaveAsync(ct);
    }
} 