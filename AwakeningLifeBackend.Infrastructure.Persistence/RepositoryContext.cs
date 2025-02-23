using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AwakeningLifeBackend.Infrastructure.Persistence;

public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BaseEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DependantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionFeatureConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionRoleConfiguration());
    }

    public DbSet<BaseEntity>? BaseEntities { get; set; }
    public DbSet<DependantEntity>? DependantEntities { get; set; }
    public DbSet<SubscriptionFeature>? SubscriptionFeatures { get; set; }
    public DbSet<SubscriptionRole>? SubscriptionRoles { get; set; }
}
