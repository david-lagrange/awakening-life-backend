using AutoMapper;
using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Core.Domain.Exceptions;
using AwakeningLifeBackend.Core.Services.Abstractions;
using LoggingService;
using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Web;

namespace AwakeningLifeBackend.Core.Services;

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

        return (users: usersDtos, metaData: usersWithMetaData.MetaData);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = roles.ToList();

        return userDto;
    }

    public async Task<IdentityResult> DeleteUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        if (user.UserName == "swarming-admin")
            throw new ProtectedUserException(userId);

        user.Email = $"deleted_{user.Email}_{Guid.NewGuid()}";
        user.UserName = $"deleted_{user.UserName}_{Guid.NewGuid()}";
        user.IsDeleted = true;

        user.FirstName = $"deleted_{user.FirstName}";
        user.LastName = $"deleted_{user.LastName}";
        user.PhoneNumber = null;
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;
        user.PasswordHash = null;

        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> UpdateUserAsync(Guid userId, UserForUpdateDto userForUpdateDto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        if (user.UserName == "swarming-admin")
            throw new ProtectedUserException(userId);

        user.FirstName = userForUpdateDto.FirstName;
        user.LastName = userForUpdateDto.LastName;
        user.UserName = userForUpdateDto.UserName;
        user.Email = userForUpdateDto.Email;
        user.PhoneNumber = userForUpdateDto.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded && userForUpdateDto.Roles != null)
        {
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

        if (userForUpdateDto.ChangePassword == true && !string.IsNullOrEmpty(userForUpdateDto.Password))
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetToken, userForUpdateDto.Password);

            // if (!resetPasswordResult.Succeeded)
            // {
            //     return resetPasswordResult;
            // }
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

#pragma warning disable CS8604 // Possible null reference argument.
        var user = await GetUserByEmailAsync(userForResetPasswordRequestDto.Email);
#pragma warning restore CS8604 // Possible null reference argument.

        if (user.Email == null)
        {
            throw new UserHasNoEmailSetException(new Guid(user.Id));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = $"{baseUrl}/account/reset?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(userForResetPasswordRequestDto.Email)}";

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



}

public static class RepositoryUserExtension
{
    public static IQueryable<User> Search(this IQueryable<User> user, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return user;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return user.Where(i => (i.Id != null && i.Id.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase)) ||
                               (i.FirstName != null && i.FirstName.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase)) ||
                               (i.LastName != null && i.LastName.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase)) ||
                               (i.UserName != null && i.UserName.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase)) ||
                               (i.NormalizedUserName != null && i.NormalizedUserName.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase)) ||
                               (i.Email != null && i.Email.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase)) ||
                               (i.NormalizedEmail != null && i.NormalizedEmail.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase)));
    }
}