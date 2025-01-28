﻿using AwakeningLifeBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AwakeningLifeBackend.ContextFactory;

public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var deployedEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{deployedEnv}.json")
            .Build();

        string databasePasswordEnvVar = "HOSTED_POSTGRESQL_DB_PASSWORD";

        var connectionString = configuration.GetConnectionString("sqlConnection");
        var dbPassword = Environment.GetEnvironmentVariable(databasePasswordEnvVar);

        if (connectionString == null)
        {
            throw new InvalidOperationException("Connection string 'sqlConnection' not found.");
        }

        if (dbPassword == null)
        {
            throw new InvalidOperationException($"Environment variable '{databasePasswordEnvVar}' not found.");
        }

        connectionString = connectionString.Replace("{POSTGRESQL_DB_PASSWORD}", dbPassword);

        var builder = new DbContextOptionsBuilder<RepositoryContext>()
            .UseNpgsql(connectionString, b => b.MigrationsAssembly("AwakeningLifeBackend.Infrastructure.Persistence"));

        return new RepositoryContext(builder.Options);
    }
}
