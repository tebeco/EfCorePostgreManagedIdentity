using Company.Common.EfCore;

namespace Company.AppOne.Db;

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
