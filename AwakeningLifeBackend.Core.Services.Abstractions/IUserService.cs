using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace AwakeningLifeBackend.Core.Services.Abstractions;

public interface IUserService
{
    Task<(IEnumerable<UserDto> users, MetaData metaData)> GetUsersAsync(UserParameters userParameters);
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<UserTokensDto> GetUserTokensAsync(Guid userId, string accessToken);
    Task<IdentityResult> DeleteUserAsync(Guid userId);
    Task<IdentityResult> UpdateUserAsync(Guid userId, UserForUpdateDto userForUpdateDto);
    Task SendResetPasswordEmailAsync(UserForResetPasswordRequestDto userForResetPasswordRequestDto);
    Task ResetPasswordAsync(UserForResetPasswordUpdateDto userForResetPasswordUpdateDto);
    Task SendEmailConfirmationAsync(UserForEmailConfirmationRequestDto userForEmailConfirmationRequestDto);
    Task ConfirmEmailAsync(UserForEmailConfirmationDto userForEmailConfirmationDto);
    Task UpdatePasswordAsync(Guid userId, UserForUpdatePasswordDto userForUpdatePasswordDto);
}
