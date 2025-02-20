using AutoMapper;
using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Core.Domain.Exceptions;
using AwakeningLifeBackend.Core.Services.Abstractions.Services;
using AwakeningLifeBackend.Infrastructure.ExternalServices;
using LoggingService;
using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Web;

namespace AwakeningLifeBackend.Core.Services.Services;

internal sealed class UserService : IUserService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    public UserService(ILoggerManager logger, IMapper mapper, UserManager<User> userManager, IEmailService emailService)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<(IEnumerable<UserDto> users, MetaData metaData)> GetUsersAsync(UserParameters userParameters)
    {
        _logger.LogInformation($"Retrieving users with parameters - Page: {userParameters.PageNumber}, PageSize: {userParameters.PageSize}");

        var query = _userManager.Users.Where(u => !u.IsDeleted);
        if (!string.IsNullOrEmpty(userParameters.SearchTerm))
        {
            query = query.Search(userParameters.SearchTerm);
        }
        var usersEntities = query.ToList();

        int count = usersEntities.Count;

        usersEntities = usersEntities
            .Skip((userParameters.PageNumber - 1) * userParameters.PageSize)
            .Take(userParameters.PageSize)
            .ToList();

        PagedList<User> usersWithMetaData = PagedList<User>.ToPagedList(usersEntities, count, userParameters.PageNumber, userParameters.PageSize);

        var usersDtos = new List<UserDto>();

        foreach (var user in usersEntities)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles.ToList();
            usersDtos.Add(userDto);
        }

        _logger.LogInformation($"Retrieved {usersDtos.Count} users from database");
        return (users: usersDtos, metaData: usersWithMetaData.MetaData);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        _logger.LogInformation($"Attempting to retrieve user with ID: {userId}");
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning($"User with ID: {userId} was not found");
            throw new UserNotFoundException(userId);
        }

        _logger.LogInformation($"Successfully retrieved user {user.UserName} (ID: {userId})");
        var roles = await _userManager.GetRolesAsync(user);

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = roles.ToList();

        return userDto;
    }

    public async Task<UserTokensDto> GetUserTokensAsync(Guid userId, string accessToken)
    {
        _logger.LogInformation($"Attempting to retrieve user with ID: {userId}");
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning($"User with ID: {userId} was not found");
            throw new UserNotFoundException(userId);
        }

        var userTokens = new UserTokensDto
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken
        };

        return userTokens;
    }

    public async Task<IdentityResult> DeleteUserAsync(Guid userId)
    {
        _logger.LogInformation($"Attempting to delete user with ID: {userId}");
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning($"Delete failed - User with ID: {userId} was not found");
            throw new UserNotFoundException(userId);
        }

        if (user.UserName == "swarming-admin")
        {
            _logger.LogWarning($"Attempted to delete protected user: {user.UserName}");
            throw new ProtectedUserException(userId);
        }

        user.Email = $"deleted_{user.Email}_{Guid.NewGuid()}";
        user.UserName = $"deleted_{user.UserName}_{Guid.NewGuid()}";
        user.IsDeleted = true;

        user.FirstName = $"deleted_{user.FirstName}";
        user.LastName = $"deleted_{user.LastName}";
        user.PhoneNumber = null;
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;
        user.PasswordHash = null;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            _logger.LogInformation($"Successfully deleted user {user.UserName} (ID: {userId})");
        else
            _logger.LogError($"Failed to delete user {user.UserName} (ID: {userId}). Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return result;
    }

    public async Task<IdentityResult> UpdateUserAsync(Guid userId, UserForUpdateDto userForUpdateDto)
    {
        _logger.LogInformation($"Attempting to update user with ID: {userId}");
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning($"Update failed - User with ID: {userId} was not found");
            throw new UserNotFoundException(userId);
        }

        //if (user.UserName == "")
        //    throw new ProtectedUserException(userId);

        user.FirstName = userForUpdateDto.FirstName;
        user.LastName = userForUpdateDto.LastName;
        user.UserName = userForUpdateDto.UserName;
        user.Email = userForUpdateDto.Email;
        user.PhoneNumber = userForUpdateDto.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            _logger.LogInformation($"Successfully updated user {user.UserName} (ID: {userId})");

            if (userForUpdateDto.Roles != null)
            {
                _logger.LogInformation($"Updating roles for user {user.UserName} (ID: {userId})");
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToAdd = userForUpdateDto.Roles.Except(currentRoles).ToList();
                var rolesToRemove = currentRoles.Except(userForUpdateDto.Roles).ToList();

                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
                }

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }
            }
        }
        else
        {
            _logger.LogError($"Failed to update user {user.UserName} (ID: {userId}). Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (userForUpdateDto.ChangePassword == true && !string.IsNullOrEmpty(userForUpdateDto.Password))
        {
            _logger.LogInformation($"Initiating password change for user {user.UserName} (ID: {userId})");
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetToken, userForUpdateDto.Password);

            if (!resetPasswordResult.Succeeded)
                _logger.LogError($"Password reset failed for user {user.UserName} (ID: {userId}). Errors: {string.Join(", ", resetPasswordResult.Errors.Select(e => e.Description))}");
            else
                _logger.LogInformation($"Successfully reset password for user {user.UserName} (ID: {userId})");
        }

        return result;
    }

    private async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw UserNotFoundException.CreateWithMessage($"User with email {email} not found.");
        }

        return user;
    }

    public async Task SendResetPasswordEmailAsync(UserForResetPasswordRequestDto userForResetPasswordRequestDto)
    {

        var baseUrl = Environment.GetEnvironmentVariable("API_CLIENT_REDIRECT_BASE_URL");

        if (baseUrl == null)
        {
            _logger.LogWarning("API_CLIENT_REDIRECT_BASE_URL environment variable is not set");
            throw new EnvironmentVariableNotSetException("API_CLIENT_REDIRECT_BASE_URL");
        }

#pragma warning disable CS8604 // Possible null reference argument.
        var user = await GetUserByEmailAsync(userForResetPasswordRequestDto.Email);
#pragma warning restore CS8604 // Possible null reference argument.

        if (user.Email == null)
        {
            throw new UserHasNoEmailSetException(new Guid(user.Id));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = $"{baseUrl}/auth/reset-password?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(userForResetPasswordRequestDto.Email)}";

        await _emailService.SendEmailAsync(user.Email, resetLink);
    }


    public async Task ResetPasswordAsync(UserForResetPasswordUpdateDto userForResetPasswordUpdateDto)
    {

#pragma warning disable CS8604 // Possible null reference argument.
        var user = await GetUserByEmailAsync(userForResetPasswordUpdateDto.Email);

        // Decode the token if it hasn't been decoded already
        var decodedToken = HttpUtility.UrlDecode(userForResetPasswordUpdateDto.Token);

        _logger.LogInformation($"Attempting to reset password for user {user.Email}");
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, userForResetPasswordUpdateDto.NewPassword);
#pragma warning disable CS8604 // Possible null reference argument.

        if (!result.Succeeded)
        {
            _logger.LogError($"Password reset failed for user {user.Email}. Errors: {string.Join(", ", result.Errors.Select(error => error.Description))}");
            throw new Exception($"Password reset failed. Errors: {string.Join(", ", result.Errors.Select(error => error.Description))}");
        }
    }

    public async Task SendEmailConfirmationAsync(UserForEmailConfirmationRequestDto userForEmailConfirmationRequestDto)
    {
        var baseUrl = Environment.GetEnvironmentVariable("API_CLIENT_REDIRECT_BASE_URL");

        if (baseUrl == null)
        {
            _logger.LogWarning("API_CLIENT_REDIRECT_BASE_URL environment variable is not set");
            throw new EnvironmentVariableNotSetException("API_CLIENT_REDIRECT_BASE_URL");
        }

        var user = await GetUserByEmailAsync(userForEmailConfirmationRequestDto.Email);

        if (user.Email == null)
        {
            throw new UserHasNoEmailSetException(new Guid(user.Id));
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{baseUrl}/redirects/confirm-email?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(userForEmailConfirmationRequestDto.Email)}";

        await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);
    }

    public async Task ConfirmEmailAsync(UserForEmailConfirmationDto userForEmailConfirmationDto)
    {
        var user = await GetUserByEmailAsync(userForEmailConfirmationDto.Email);
        var decodedToken = HttpUtility.UrlDecode(userForEmailConfirmationDto.Token);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            _logger.LogError($"Email confirmation failed for user {user.Email}. Errors: {string.Join(", ", result.Errors.Select(error => error.Description))}");
            throw new Exception($"Email confirmation failed. Errors: {string.Join(", ", result.Errors.Select(error => error.Description))}");
        }
    }

    public async Task UpdatePasswordAsync(Guid userId, UserForUpdatePasswordDto userForUpdatePasswordDto)
    {
        _logger.LogInformation($"Attempting to update password for user with ID: {userId}");
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning($"Update password failed - User with ID: {userId} was not found");
            throw new UserNotFoundException(userId);
        }

        // Verify the current password
        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, userForUpdatePasswordDto.CurrentPassword);
        if (!isCurrentPasswordValid)
        {
            _logger.LogWarning($"Update password failed - Current password is invalid for user ID: {userId}");
            throw new UpdatePasswordBadRequest("Current password is incorrect");
        }

        // Change the password
        var result = await _userManager.ChangePasswordAsync(user, userForUpdatePasswordDto.CurrentPassword, userForUpdatePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            _logger.LogError($"Failed to update password for user {user.UserName} (ID: {userId}). Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            throw new UpdatePasswordBadRequest($"Password update failed. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        _logger.LogInformation($"Successfully updated password for user {user.UserName} (ID: {userId})");
    }
}

public static class RepositoryUserExtension
{
    public static IQueryable<User> Search(this IQueryable<User> user, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return user;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return user.Where(i => i.Id != null && i.Id.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase) ||
                               i.FirstName != null && i.FirstName.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase) ||
                               i.LastName != null && i.LastName.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase) ||
                               i.UserName != null && i.UserName.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase) ||
                               i.NormalizedUserName != null && i.NormalizedUserName.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase) ||
                               i.Email != null && i.Email.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase) ||
                               i.NormalizedEmail != null && i.NormalizedEmail.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase));
    }
}