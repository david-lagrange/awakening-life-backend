using AwakeningLifeBackend.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace AwakeningLifeBackend.Infrastructure.Presentation.Controllers;

[Route("api/contact")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly IServiceManager _service;

    public ContactController(IServiceManager service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> SendContactMessage([FromBody] ContactDto contactDto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.ContactService.SendContactMessageAsync(contactDto, ct);
        
        if (!result)
            return StatusCode(500, "Failed to send your message. Please try again later.");
            
        return Ok();
    }
} 