using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record ContactDto
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; init; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; init; }
    
    [Required(ErrorMessage = "Subject is required")]
    public required string Subject { get; init; }
    
    [Required(ErrorMessage = "Message is required")]
    public required string Message { get; init; }
} 