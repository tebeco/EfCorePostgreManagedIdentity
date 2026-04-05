using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Company.Common.Api;

public static class DbContextHostApplicationBuilderExtensionsasdf
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapCommonEndpoints()
        {
            endpoints.MapOpenApi();
            endpoints.MapScalarApiReference();

            return endpoints;
        }
    }
}