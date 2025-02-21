using AwakeningLifeBackend.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AwakeningLifeBackend.Infrastructure.Persistence.Configurations;
public class SubscriptionFeatureConfiguration : IEntityTypeConfiguration<SubscriptionFeature>
{
    public void Configure(EntityTypeBuilder<SubscriptionFeature> builder)
    {
        builder.HasKey(h => h.SubscriptionFeatureId);
    }
}