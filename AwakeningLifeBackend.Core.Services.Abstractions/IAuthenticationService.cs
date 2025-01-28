﻿using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;

namespace AwakeningLifeBackend.Core.Services.Abstractions;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);
    Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
    Task<TokenDto> CreateToken(bool populateExp);
    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}