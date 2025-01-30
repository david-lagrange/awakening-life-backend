using AwakeningLifeBackend.Core.Services.Abstractions;
using AwakeningLifeBackend.Infrastructure.Presentation.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace AwakeningLifeBackend.Infrastructure.Presentation.Controllers;

[Route("api/users")]
[ApiController]

public class UsersController : ControllerBase
{
    private readonly IServiceManager _service;

    public UsersController(IServiceManager service) => _service = service;

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

    [HttpPost("reset-password")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> ResetPasswordRequest([FromBody] UserForResetPasswordRequestDto userForResetPasswordDto)
    {
        await _service.UserService.SendResetPasswordEmailAsync(userForResetPasswordDto);

        return Ok();
    }

    [HttpPut("reset-password")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> ResetPasswordUpdate([FromBody] UserForResetPasswordUpdateDto userForResetPasswordDto)
    {
        await _service.UserService.ResetPasswordAsync(userForResetPasswordDto);

        return Ok();
    }

    [HttpPost("confirm-email-request")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> ConfirmEmailRequest([FromBody] UserForEmailConfirmationRequestDto confirmationRequestDto)
    {
        await _service.UserService.SendEmailConfirmationAsync(confirmationRequestDto);
        return Ok();
    }

    [HttpPut("confirm-email")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> ConfirmEmail([FromBody] UserForEmailConfirmationDto confirmationDto)
    {
        await _service.UserService.ConfirmEmailAsync(confirmationDto);
        return Ok();
    }

}
