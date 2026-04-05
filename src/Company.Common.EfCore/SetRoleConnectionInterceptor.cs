using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace Company.Common.EfCore;

public class SetRoleConnectionInterceptor<TDbContextOptions>(IOptions<TDbContextOptions> options) : DbConnectionInterceptor
    where TDbContextOptions : class, ICommonDbContextOptions
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
