using AwakeningLifeBackend.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(new User
        {
            Id = "c78cca8c-efa3-4524-ae21-98904dadf303",
            FirstName = "Equanimity",
            LastName = "Admin",
            UserName = "equanimity-admin",
            NormalizedUserName = "EQUANIMITY-ADMIN",
            Email = "admin@equanimity-solutions.com",
            NormalizedEmail = "ADMIN@EQUANIMITY-SOLUTIONS.COM",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEBabxi2j4ViaUyJTawKOoncZJcJUvtnGSMQStB+D9Qqz8Yc/HfbNvCIW6FUvDM2tPw==",
            SecurityStamp = "388fe995-3763-4c99-848b-8699c3131d0d",
            ConcurrencyStamp = "b8baa1c8-b7eb-4909-bbd5-a3e305a4381e"
        });
    }
}
