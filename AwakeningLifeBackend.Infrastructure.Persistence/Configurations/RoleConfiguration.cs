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
            },
            new IdentityRole
            {
                Id = "6345c99d-8cce-45f1-a876-8a99fe91cd94",
                Name = "View Roles",
                NormalizedName = "VIEW ROLES"
            },
            new IdentityRole
            {
                Id = "5be06b26-d163-4027-84b9-31155c61393b",
                Name = "View IP",
                NormalizedName = "VIEW IP"
            },
            new IdentityRole
            {
                Id = "b9b0109e-a82f-4934-a12c-dd1ba0c87f04",
                Name = "Contemplation",
                NormalizedName = "CONTEMPLATION"
            },
            new IdentityRole
            {
                Id = "6bdc6e0c-dc95-4e90-8f4b-50bf684dff71",
                Name = "Mind Clearing",
                NormalizedName = "MIND CLEARING"
            },
            new IdentityRole
            {
                Id = "310071fe-00bc-41f1-9161-10a9879fa91c",
                Name = "Deepest Vision",
                NormalizedName = "DEEPEST VISION"
            },
            new IdentityRole
            {
                Id = "95746f8b-6570-4b53-914f-2d5639be416e",
                Name = "Manifestation",
                NormalizedName = "MANIFESTATION"
            },
            new IdentityRole
            {
                Id = "03b4f3a4-d2a6-4e6f-86c1-d5a0b8d027f7",
                Name = "Gratitude & Goals",
                NormalizedName = "GRATITUDE & GOALS"
            },
            new IdentityRole
            {
                Id = "902df002-5302-4055-a1c7-39bcdb14ea40",
                Name = "Custom Journey",
                NormalizedName = "CUSTOM JOURNEY"
            },
            new IdentityRole
            {
                Id = "edda87cb-fe80-4b82-9a74-8e992f049423",
                Name = "Technique Training",
                NormalizedName = "TECHNIQUE TRAINING"
            }
        );
    }
}