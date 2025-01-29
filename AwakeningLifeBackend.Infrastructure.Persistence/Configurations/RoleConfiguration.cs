using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "1fca31b0-e385-46b3-98f2-13f47864589b",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new IdentityRole
            {
                Id = "62227b59-0b0f-43a3-834e-5e2cd3a60ea7",
                Name = "Modify Users",
                NormalizedName = "MODIFY USERS"
            },
            new IdentityRole
            {
                Id = "77e345c0-86e5-4aa6-9744-54b83db0b53d",
                Name = "View Users",
                NormalizedName = "VIEW USERS"
            }
        );
    }
}