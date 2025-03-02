using System;

namespace AwakeningLifeBackend.Core.Domain.Exceptions;

public class WaitlistNotFoundException : NotFoundException
{
    public WaitlistNotFoundException(Guid waitlistId)
        : base($"The waitlist entry with id: {waitlistId} doesn't exist in the database.")
    {
    }
} 