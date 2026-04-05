using Azure.Core;
using Azure.Identity;
using Company.AppOne.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Company.Common.EfCore;

public static class OneDbContextHostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddOneDbContext()
        {
            builder.AddCommonDbContext<OneDbContext, OneDbContextOptions>();

            return builder;
        }
    }
}
