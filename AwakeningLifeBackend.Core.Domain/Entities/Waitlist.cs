using System;

namespace AwakeningLifeBackend.Core.Domain.Entities;

public class Waitlist
{
    public Guid WaitlistId { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 