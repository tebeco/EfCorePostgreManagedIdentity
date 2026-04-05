using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Company.Common.EfCore;

public static class DbContextHostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddCommonDbContext<TDbContext, TDbContextOptions>()
            where TDbContext : DbContext
            where TDbContextOptions : class, ICommonDbContextOptions
        {
            builder.Services.AddOptionsWithValidateOnStart<TDbContextOptions>()
                .BindConfiguration(TDbContextOptions.SectionName)
                .PostConfigure(options =>
                {
                    options.Port = options.Port == 0 ? 5432 : options.Port;
                    options.CommandTimeout = options.CommandTimeout == TimeSpan.Zero ? TimeSpan.FromSeconds(60) : options.CommandTimeout;

                    options.AzureCredentials?.Scope ??= $"https://ossrdbms-aad.database.windows.net/.default";
                    options.AzureCredentials?.ClientId = string.IsNullOrEmpty(options.AzureCredentials?.ClientId) ? null : options.AzureCredentials.ClientId;
                })
                .Validate(options => !string.IsNullOrWhiteSpace(options.Host), $"{TDbContextOptions.SectionName}:{nameof(ICommonDbContextOptions.Host)} cannot be null or whitespace")
                .Validate(options => !string.IsNullOrWhiteSpace(options.Database), $"{TDbContextOptions.SectionName}:{nameof(ICommonDbContextOptions.Database)} cannot be null or whitespace")

                .Validate(options => options.CredentialType != CredentialType.PlainText || options.PlainTextCredentials != null, $"{TDbContextOptions.SectionName}:{nameof(ICommonDbContextOptions.PlainTextCredentials)} cannot be null when CredentialType=PlainText")
                .Validate(options => options.CredentialType != CredentialType.PlainText || !string.IsNullOrWhiteSpace(options.PlainTextCredentials?.Username), $"{TDbContextOptions.SectionName}:{nameof(ICommonDbContextOptions.PlainTextCredentials)}:{nameof(PlainTextCredentials.Username)} cannot be null or whitespace")
                .Validate(options => options.CredentialType != CredentialType.PlainText || options.PlainTextCredentials?.Password != null, $"{TDbContextOptions.SectionName}:{nameof(ICommonDbContextOptions.PlainTextCredentials)}:{nameof(PlainTextCredentials.Password)} cannot be null or whitespace")

                .Validate(options => options.CredentialType != CredentialType.AzureManagedIdentity || options.AzureCredentials != null, $"{TDbContextOptions.SectionName}:{nameof(ICommonDbContextOptions.AzureCredentials)} cannot be null when CredentialType=AzureManagedIdentity")
                .Validate(options => options.CredentialType != CredentialType.AzureManagedIdentity || !string.IsNullOrWhiteSpace(options.AzureCredentials?.TenantId), $"{TDbContextOptions.SectionName}:{nameof(ICommonDbContextOptions.AzureCredentials)}:{nameof(PostgreManagedIdentityCredentials.TenantId)} cannot be null or whitespace")
                .Validate(options => options.CredentialType != CredentialType.AzureManagedIdentity || !string.IsNullOrWhiteSpace(options.AzureCredentials?.Scope), $"{TDbContextOptions.SectionName}:{nameof(ICommonDbContextOptions.AzureCredentials)}:{nameof(PostgreManagedIdentityCredentials.Scope)} cannot be null or whitespace")
                .Validate(options => options.CredentialType != CredentialType.AzureManagedIdentity || !string.IsNullOrWhiteSpace(options.AzureCredentials?.Username), $"{TDbContextOptions.SectionName}:{nameof(ICommonDbContextOptions.AzureCredentials)}:{nameof(PostgreManagedIdentityCredentials.Username)} cannot be null or whitespace")
                ;

            builder.Services.AddNpgsqlDataSource(
                null!,
                (serviceProvider, npgsqlDataSourceBuilder) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<TDbContextOptions>>().Value;
                    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                    npgsqlDataSourceBuilder.UseLoggerFactory(loggerFactory);

                    var connectionStringBuilder = npgsqlDataSourceBuilder.ConnectionStringBuilder;
                    connectionStringBuilder.Host = options.Host;
                    connectionStringBuilder.Port = options.Port;
                    connectionStringBuilder.Database = options.Database;
                    connectionStringBuilder.CommandTimeout = (int)options.CommandTimeout.TotalSeconds;
                    connectionStringBuilder.SslMode = SslMode.Prefer;

                    if (options.CredentialType == CredentialType.PlainText)
                    {
                        connectionStringBuilder.Username = options.PlainTextCredentials!.Username;
                        connectionStringBuilder.Password = options.PlainTextCredentials!.Password;
                    }
                    else
                    {
                        connectionStringBuilder.Username = options.AzureCredentials!.Username;
                        connectionStringBuilder.Password = null;
                        npgsqlDataSourceBuilder
                            .UsePeriodicPasswordProvider(
                                async (connectionStringBuilder, cancellationToken) =>
                                {
                                    var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = options.AzureCredentials!.ClientId });
                                    var token = await credentials.GetTokenAsync(new TokenRequestContext([options.AzureCredentials!.Scope]), cancellationToken);

                                    return token.Token;
                                },
                                options.AzureCredentials!.SuccessRefreshInterval,
                                options.AzureCredentials!.FailureRefreshInterval);
                    }
                });


            // Make sure that the connection apply role in order to be granted access to the database objects
            // SET ROLE "app_one";
            builder.Services.AddTransient<SetRoleConnectionInterceptor<TDbContextOptions>>();

            builder.Services.AddDbContext<TDbContext>((sp, dbContextOptionsBuilder) =>
            {
                var npgsqlDataSource = sp.GetRequiredService<NpgsqlDataSource>();

                if(sp.GetRequiredService<IOptions<TDbContextOptions>>().Value.Role is not null)
                {
                    var setRoleConnectionInterceptor = sp.GetRequiredService<SetRoleConnectionInterceptor<TDbContextOptions>>();
                    dbContextOptionsBuilder.AddInterceptors(setRoleConnectionInterceptor);
                }

                dbContextOptionsBuilder.UseNpgsql(npgsqlDataSource);
            });

            return builder;
        }
    }
}
