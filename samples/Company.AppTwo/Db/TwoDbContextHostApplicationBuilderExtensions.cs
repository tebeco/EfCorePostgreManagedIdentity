using Company.Common.EfCore;

namespace Company.AppTwo.Db;

public static class TwoDbContextHostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddTwoDbContext()
        {
            builder.AddCommonDbContext<TwoDbContext, TwoDbContextOptions>();

            return builder;
        }
    }
}
