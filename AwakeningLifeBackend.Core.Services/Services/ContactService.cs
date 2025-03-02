using AwakeningLifeBackend.Core.Services.Abstractions.Services;
using AwakeningLifeBackend.Infrastructure.ExternalServices;
using LoggingService;
using Shared.DataTransferObjects;
using System.Threading;
using System.Threading.Tasks;

namespace AwakeningLifeBackend.Core.Services.Services;

internal sealed class ContactService : IContactService
{
    private readonly ILoggerManager _logger;
    private readonly IEmailService _emailService;
    private readonly string _adminEmail = "david@equanimity-solutions.com";

    public ContactService(ILoggerManager logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<bool> SendContactMessageAsync(ContactDto contactDto, CancellationToken ct = default)
    {
        _logger.LogInformation($"Sending contact form message from {contactDto.Email}");
        
        try
        {
            await _emailService.SendContactFormEmailAsync(
                _adminEmail,
                contactDto.Name,
                contactDto.Email,
                contactDto.Subject,
                contactDto.Message);
                
            return true;
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"Error sending contact form email: {ex.Message}");
            return false;
        }
    }
} 