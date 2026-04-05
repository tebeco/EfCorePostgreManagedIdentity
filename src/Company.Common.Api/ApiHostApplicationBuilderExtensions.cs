using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Company.Common.Api;

public static class DbContextHostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddCommonApi()
        {
            builder.Services.AddOpenApi(options => options.AddScalarTransformers());
            builder.Services.Configure<ScalarOptions>(options => options.DisableAgent());

            return builder;
        }
    }
}