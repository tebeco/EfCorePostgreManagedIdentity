using Company.Common.EfCore;

namespace Company.AppTwo.Db;

public class TwoDbContextOptions : ICommonDbContextOptions
{
    public static string SectionName { get; set; } = "TwoDbContext";

    public required string Host { get; set; } = "localhost";

    public required int Port { get; set; } = 5432;

    public required string Database { get; set; } = "app_two_db";

    public required TimeSpan CommandTimeout { get; set; }

    public string? Role { get; set; }

    public required CredentialType CredentialType { get; set; }

    public PlainTextCredentials? PlainTextCredentials { get; set; }

    public PostgreManagedIdentityCredentials? AzureCredentials { get; set; }
}