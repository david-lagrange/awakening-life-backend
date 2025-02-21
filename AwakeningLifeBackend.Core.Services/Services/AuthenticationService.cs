using AutoMapper;
using AwakeningLifeBackend.Core.Domain.Entities;
using LoggingService;
using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;
using AwakeningLifeBackend.Core.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using AwakeningLifeBackend.Core.Domain.ConfigurationModels;
using Microsoft.Extensions.Options;
using AwakeningLifeBackend.Core.Services.Abstractions.Services;
using AwakeningLifeBackend.Infrastructure.ExternalServices;
using Stripe;

namespace AwakeningLifeBackend.Core.Services.Services;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IOptions<JwtConfiguration> _configuration;
    private readonly JwtConfiguration _jwtConfiguration;
    private User? _user;
    private readonly IStripeService _stripeService;

    public AuthenticationService(ILoggerManager logger, IMapper mapper,
        UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JwtConfiguration> configuration, IStripeService stripeService)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _jwtConfiguration = new JwtConfiguration();
        _jwtConfiguration = _configuration.Value;
        _stripeService = stripeService;
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        _logger.LogInformation($"Attempting to register new user with email: {userForRegistration.Email}");

        foreach (var role in userForRegistration.Roles!)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                _logger.LogError($"Registration failed. Role not found: {role}");
                throw new RoleNotFoundException(role);
            }
        }

        var user = _mapper.Map<User>(userForRegistration);

        if (string.IsNullOrEmpty(user.UserName))
        {
            var emailPrefix = userForRegistration.Email!.Split('@')[0];
            user.UserName = emailPrefix;
        }

        user.UserName = $"{user.UserName}_{Guid.NewGuid().ToString().ToLower().Substring(0, 5)}";

        try
        {
            _logger.LogInformation($"Creating new Stripe customer for user {user.Email}");
            var stripeCustomer = await _stripeService.CreateCustomerAsync(user.Email);

            user.StripeCustomerId = stripeCustomer.Id;
        }
        catch (StripeException ex)
        {
            _logger.LogError($"Failed to create Stripe customer for user {user.Email}. Error: {ex.Message}");
        }

        var result = await _userManager.CreateAsync(user, userForRegistration.Password!);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles!);
            _logger.LogInformation($"Successfully registered new user. Username: {user.UserName}, Email: {user.Email}, Roles: {string.Join(", ", userForRegistration.Roles)}");
        }
        else
        {
            _logger.LogError($"User registration failed for email: {userForRegistration.Email}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return result;
    }

    public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
    {
        _user = await _userManager.FindByEmailAsync(userForAuth.Email!);

        var result = _user != null && await _userManager.CheckPasswordAsync(_user, userForAuth.Password!);

        if (!result)
            _logger.LogWarning($"{nameof(ValidateUser)}: " +
                $"Authentication failed. Wrong email or password.");

        return result;
    }

    public async Task<TokenDto> CreateToken(bool populateExp)
    {
        if (_user == null)
        {
            throw new InvalidOperationException("AuthService CreateToken - User is not set.");
        }

        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();

        _user.RefreshToken = refreshToken;

        if (populateExp)
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        try
        {
            if (string.IsNullOrEmpty(_user.StripeCustomerId))
            {
                _logger.LogInformation($"Creating new Stripe customer for user {_user.UserName}");
#pragma warning disable CS8604 // Possible null reference argument.
                var stripeCustomer = await _stripeService.CreateCustomerAsync(_user.Email);
#pragma warning restore CS8604 // Possible null reference argument.

                _user.StripeCustomerId = stripeCustomer.Id;
            }
        }
        catch (StripeException ex)
        {
            _logger.LogError($"Failed to create Stripe customer for user {_user.Email}. Error: {ex.Message}");
        }

        await _userManager.UpdateAsync(_user);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, refreshToken);
    }
    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        if (tokenDto.AccessToken == null)
        {
            throw new ArgumentNullException(nameof(tokenDto.AccessToken));
        }

        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        var userName = principal.Identity?.Name;

        if (userName == null)
        {
            throw new RefreshTokenBadRequestException();
        }

        var user = await _userManager.FindByNameAsync(userName);
        if (user == null || user.RefreshToken != tokenDto.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new RefreshTokenBadRequestException();
        }

        _user = user;

        return await CreateToken(populateExp: false);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("API_JWT_SECRET")!);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim("userId", _user!.Id!),
            new Claim(ClaimTypes.Name, _user!.UserName!),
            new Claim("username", _user!.UserName!),
            new Claim("email", _user!.Email!),
            new Claim("emailConfirmed", _user!.EmailConfirmed.ToString()!),
        };

        var roles = await _userManager.GetRolesAsync(_user);

        foreach (var role in roles)
            claims.Add(new Claim("roles", role));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
            signingCredentials: signingCredentials
        );

        return tokenOptions;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("API_JWT_SECRET")!)),
            ValidateLifetime = true,
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}