using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(
            new IdentityUserRole<string> { UserId = "c78cca8c-efa3-4524-ae21-98904dadf303", RoleId = "1fca31b0-e385-46b3-98f2-13f47864589b" }
        );
    }
}
