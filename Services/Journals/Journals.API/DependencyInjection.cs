using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using Articles.Security;
using Blocks.Core;
using FastEndpoints;
using FastEndpoints.Swagger;
using Blocks.Mapster;
using Blocks.AspNetCore.Grpc;
using Auth.Grpc;
using ProtoBuf.Grpc.Server;
using System.IO.Compression;
using Blocks.Core.Security;
using Blocks.AspNetCore.Providers;

namespace Journals.API;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApiOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAndValidateOptions<JwtOptions>(configuration)
            .Configure<JsonOptions>(opt =>
            {
                opt.SerializerOptions.PropertyNameCaseInsensitive = true;
                opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddFastEndpoints()
            .SwaggerDocument()
            .AddJwtAuthentication(config)
            .AddMapsterConfigsFromCurrentAssembly()
            .AddAuthorization()
            ;

        services
            .AddScoped<IClaimsProvider, HttpContextProvider>()
            .AddScoped<HttpContextProvider>();

        //server
        services.AddCodeFirstGrpc(options =>
        {
            options.ResponseCompressionLevel = CompressionLevel.Fastest;
            options.EnableDetailedErrors = true;
            options.Interceptors.Add<AssignUserIdInterceptor>();
        });

        //clients
        var grpcOptions = config.GetSectionByTypeName<GrpcServicesOptions>();
        services.AddCodeFirstGrpcClient<IPersonService>(grpcOptions, "Person");

        return services;
    }
}
