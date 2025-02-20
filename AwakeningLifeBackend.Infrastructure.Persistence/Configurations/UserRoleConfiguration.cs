using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(
            new IdentityUserRole<string> { UserId = "c78cca8c-efa3-4524-ae21-98904dadf303", RoleId = "1fca31b0-e385-46b3-98f2-13f47864589b" },
            new IdentityUserRole<string> { UserId = "c78cca8c-efa3-4524-ae21-98904dadf303", RoleId = "62227b59-0b0f-43a3-834e-5e2cd3a60ea7" },
            new IdentityUserRole<string> { UserId = "c78cca8c-efa3-4524-ae21-98904dadf303", RoleId = "77e345c0-86e5-4aa6-9744-54b83db0b53d" },
            new IdentityUserRole<string> { UserId = "c78cca8c-efa3-4524-ae21-98904dadf303", RoleId = "6345c99d-8cce-45f1-a876-8a99fe91cd94" },
            new IdentityUserRole<string> { UserId = "c78cca8c-efa3-4524-ae21-98904dadf303", RoleId = "5be06b26-d163-4027-84b9-31155c61393b" }
        );
    }
}
