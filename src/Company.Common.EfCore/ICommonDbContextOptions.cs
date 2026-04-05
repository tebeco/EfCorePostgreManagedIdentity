namespace Company.Common.EfCore;

public interface ICommonDbContextOptions
{
    static abstract string SectionName { get; set; }

    string Host { get; set; }

    int Port { get; set; }

    string Database { get; set; }

    TimeSpan CommandTimeout { get; set; }

    string? Role { get; set; }

    CredentialType CredentialType { get; set; }

    PlainTextCredentials? PlainTextCredentials { get; set; }

    PostgreManagedIdentityCredentials? AzureCredentials { get; set; }
}

public enum CredentialType
{
    PlainText,
    AzureManagedIdentity
}

public class PlainTextCredentials
{
    public required string Username { get; set; }

    public required string Password { get; set; }
}

public class PostgreManagedIdentityCredentials
{
    public required string Username { get; set; }

    public required string? ClientId { get; set; }

    public required string TenantId { get; set; }

    public required string Scope { get; set; }

    public required TimeSpan SuccessRefreshInterval { get; set; }

    public required TimeSpan FailureRefreshInterval { get; set; }
}