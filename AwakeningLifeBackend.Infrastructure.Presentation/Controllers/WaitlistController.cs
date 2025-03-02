using AwakeningLifeBackend.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AwakeningLifeBackend.Infrastructure.Presentation.Controllers;

[Route("api/waitlist")]
[ApiController]
public class WaitlistController : ControllerBase
{
    private readonly IServiceManager _service;

    public WaitlistController(IServiceManager service) => _service = service;

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAllWaitlistEntries(CancellationToken ct = default)
    {
        var waitlistEntries = await _service.WaitlistService.GetAllWaitlistEntriesAsync(false, ct);
        return Ok(waitlistEntries);
    }

    [HttpGet("{id:guid}", Name = "GetWaitlistEntryById")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetWaitlistEntryById(Guid id, CancellationToken ct = default)
    {
        var waitlistEntry = await _service.WaitlistService.GetWaitlistEntryByIdAsync(id, false, ct);
        return Ok(waitlistEntry);
    }

    [HttpPost]
    [AllowAnonymous] // Allow anyone to join the waitlist
    public async Task<IActionResult> CreateWaitlistEntry([FromBody] WaitlistForCreationDto waitlist, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdWaitlistEntry = await _service.WaitlistService.CreateWaitlistEntryAsync(waitlist, ct);
        
        return CreatedAtRoute("GetWaitlistEntryById", new { id = createdWaitlistEntry.WaitlistId }, createdWaitlistEntry);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteWaitlistEntry(Guid id, CancellationToken ct = default)
    {
        await _service.WaitlistService.DeleteWaitlistEntryAsync(id, ct);
        return NoContent();
    }
} 