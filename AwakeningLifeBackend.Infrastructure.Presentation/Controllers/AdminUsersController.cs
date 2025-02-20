using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AwakeningLifeBackend.Core.Services.Abstractions;
using AwakeningLifeBackend.Infrastructure.Presentation.ActionFilters;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace AwakeningLifeBackend.Infrastructure.Presentation.Controllers;

[Route("api/admin/users")]
[ApiController]

public class AdminUsersController : ControllerBase
{
    private readonly IServiceManager _service;

    public AdminUsersController(IServiceManager service) => _service = service;

    [HttpPost]
    [Authorize(Roles = "View Users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] UserParameters userParameters)
    {
        var pagedResult = await _service.UserService.GetUsersAsync(userParameters);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.users);
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "View Users")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var user = await _service.UserService.GetUserByIdAsync(userId);

        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Modify Users")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateUser(Guid id, UserForUpdateDto userForUpdateDto)
    {
        var result = await _service.UserService.UpdateUserAsync(id, userForUpdateDto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Modify Users")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _service.UserService.DeleteUserAsync(id);

        return NoContent();
    }

}

