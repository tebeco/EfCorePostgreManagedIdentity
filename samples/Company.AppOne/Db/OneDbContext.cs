using Company.AppOne.Db.Entities;
using Company.Common.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Company.AppOne.Db;

public class OneDbContext(DbContextOptions<OneDbContext> options) : DbContext(options)
{
    public required DbSet<Foo> Foos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}

public class OneDbContextOptions : ICommonDbContextOptions
{
    public static string SectionName { get; set; } = "OneDbContext";

    public required string Host { get; set; } = "localhost";

    public required int Port { get; set; } = 5432;

    public required string Database { get; set; } = "app_one_db";

    public required TimeSpan CommandTimeout { get; set; }

    public string? Role { get; set; }

    public required CredentialType CredentialType { get; set; }

    public PlainTextCredentials? PlainTextCredentials { get; set; }

    public PostgreManagedIdentityCredentials? AzureCredentials { get; set; }
}