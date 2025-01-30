using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record UserForAuthenticationDto
{
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; init; }
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; init; }
}

public record UserForRegistrationDto
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    [Required(ErrorMessage = "Username is required")]
    public string? UserName { get; init; }
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public ICollection<string>? Roles { get; init; }
}
public record UserDto
{
    public string? Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public ICollection<string>? Roles { get; set; }
}

public record UserForUpdateDto
{
    [Required(ErrorMessage = "FirstName is required")]
    public string? FirstName { get; init; }
    [Required(ErrorMessage = "LastName is required")]
    public string? LastName { get; init; }
    [Required(ErrorMessage = "Username is required")]
    public string? UserName { get; init; }
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public bool? ChangePassword { get; init; }
    public string? Password { get; init; }
    public ICollection<string>? Roles { get; set; }
}

public record UserForResetPasswordUpdateDto
{
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; init; }
    [Required(ErrorMessage = "Token is required")]
    public string? Token { get; init; }
    [Required(ErrorMessage = "NewPassword is required")]
    public string? NewPassword { get; init; }
}

public record UserForResetPasswordRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; init; }
}
