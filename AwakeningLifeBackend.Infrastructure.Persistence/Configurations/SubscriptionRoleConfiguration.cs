using AwakeningLifeBackend.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Configurations;

public class SubscriptionRoleConfiguration : IEntityTypeConfiguration<SubscriptionRole>
{
    public void Configure(EntityTypeBuilder<SubscriptionRole> builder)
    {
        // Configure composite key
        builder.HasKey(sr => new { sr.ProductId, sr.RoleId });

        // Configure foreign key relationship
        builder.HasOne(sr => sr.Role)
            .WithMany()
            .HasForeignKey(sr => sr.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ProductId property
        builder.Property(sr => sr.ProductId)
            .IsRequired()
            .HasMaxLength(100);  // Adjust max length as needed
    }
} 