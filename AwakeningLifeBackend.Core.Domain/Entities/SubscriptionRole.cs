using Microsoft.AspNetCore.Identity;

namespace AwakeningLifeBackend.Core.Domain.Entities;

public class SubscriptionRole
{
    public required string ProductId { get; set; }
    public required string RoleId { get; set; }
    
    public virtual IdentityRole? Role { get; set; }
}
