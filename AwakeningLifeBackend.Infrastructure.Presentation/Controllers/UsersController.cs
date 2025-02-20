using AwakeningLifeBackend.Core.Services.Abstractions;
using AwakeningLifeBackend.Infrastructure.Presentation.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace AwakeningLifeBackend.Infrastructure.Presentation.Controllers
{
    [Route("api/users")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UsersController(IServiceManager service) => _service = service;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var userId = User.FindFirst("userId")?.Value;

            var user = await _service.UserService.GetUserByIdAsync(Guid.Parse(userId ?? ""));
            return Ok(user);
        }

        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateUser(UserForUpdateDto userForUpdateDto)
        {
            var userId = User.FindFirst("userId")?.Value;

            var result = await _service.UserService.UpdateUserAsync(Guid.Parse(userId ?? ""), userForUpdateDto);

            return NoContent();
        }

        //[HttpPut("subscription")]
        //[Authorize]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //public async Task<IActionResult> UpdateUserSubscription(UserSubscriptionUpdateDto userSubscriptionUpdateDto)
        //{
        //    var userId = User.FindFirst("userId")?.Value;

        //    await _service.UserService.UpdateUserSubscriptionAsync(Guid.Parse(userId ?? ""), userSubscriptionUpdateDto);

        //    return NoContent();
        //}

        [HttpPut("update-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdatePassword([FromBody] UserForUpdatePasswordDto userForUpdatePasswordDto)
        {
            var userId = User.FindFirst("userId")?.Value;

            await _service.UserService.UpdatePasswordAsync(Guid.Parse(userId ?? ""), userForUpdatePasswordDto);

            return Ok();
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
}
