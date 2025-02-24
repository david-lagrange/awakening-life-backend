using AwakeningLifeBackend.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Configurations;

public class SubscriptionCancelationConfiguration : IEntityTypeConfiguration<SubscriptionCancelation>
{
    public void Configure(EntityTypeBuilder<SubscriptionCancelation> builder)
    {
        builder.HasKey(sc => sc.SubscriptionCancelationId);

        builder.Property(sc => sc.SubscriptionId)
            .IsRequired();

        builder.Property(sc => sc.CancelationDate)
            .IsRequired();
    }
} 