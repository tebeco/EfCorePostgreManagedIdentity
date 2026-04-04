using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

using Npgsql;

namespace Company.AppOne.Db;

public static class OneDbContextHostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddCommoneDbContext<TDbContext, TDbContextOptions>()
            where TDbContext : DbContext
            where TDbContextOptions : ICommonDbContextOptions
        {
            // Make sure that the connection apply role in order to be granted access to the database objects
            // SET ROLE "app_one";
            builder.Services.AddTransient<SetRoleConnectionInterceptor>();
            
            builder.Services.AddDbContext<TDbContext>((sp, dbContextOptionsBuilder) =>
            {
                var npgsqlDataSource = sp.GetRequiredService<NpgsqlDataSource>();
                var setRoleConnectionInterceptor = sp.GetRequiredService<SetRoleConnectionInterceptor>();

                dbContextOptionsBuilder.AddInterceptors(setRoleConnectionInterceptor);
                dbContextOptionsBuilder.UseNpgsql(npgsqlDataSource);
            });
            
            return builder;
        }
    }
}

public interface ICommonDbContextOptions
{
    string Role { get; set; }
}

public class SetRoleConnectionInterceptor<TDbContextOptions>(IOptions<TDbContextOptions> options) : DbConnectionInterceptor
    where TDbContextOptions : ICommonDbContextOptions
{
    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(options.Value.Role))
        {
            var command = connection.CreateCommand();
            command.CommandText = $"""
                                   SET ROLE "{options.Value.Role}";
                                   """;
            
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
