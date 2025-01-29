using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace AwakeningLifeBackend.Core.Services.Abstractions;

public interface IUserService
{
    Task<(IEnumerable<UserDto> users, MetaData metaData)> GetUsersAsync(UserParameters userParameters);
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<IdentityResult> DeleteUserAsync(Guid userId);
    Task<IdentityResult> UpdateUserAsync(Guid userId, UserForUpdateDto userForUpdateDto);
    Task SendResetPasswordEmailAsync(UserForResetPasswordRequestDto userForResetPasswordRequestDto);
    Task ResetPasswordAsync(UserForResetPasswordUpdateDto userForResetPasswordUpdateDto);
}
