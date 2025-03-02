using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record WaitlistDto
{
    public Guid WaitlistId { get; init; }
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record WaitlistForCreationDto
{
    [Required(ErrorMessage = "Email is required")]
    public required string Email { get; init; }
} 