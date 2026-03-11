using Articles.Security;
using Auth.Grpc;
using Blocks.AspNetCore.Grpc;
using Blocks.Core;
using Blocks.Messaging;
using FileStorage.MongoGridFS;
using Journals.Grpc;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

namespace Submission.API;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApiOptions(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddAndValidateOptions<RabbitMqOptions>(config)
            .AddAndValidateOptions<JwtOptions>(config)
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
            .AddMemoryCache()               // For caching purposes, if needed
            .AddEndpointsApiExplorer()      // For API endpoint discovery in Swagger
            .AddSwaggerGen()                // For generating Swagger documentation
            ;

        services.AddMongoFileStorage(config);

        var grpcOptions = config.GetSectionByTypeName<GrpcServicesOptions>();
        services.AddCodeFirstGrpcClient<IPersonService>(grpcOptions, "Person");
        services.AddCodeFirstGrpcClient<IJournalService>(grpcOptions, "Journal");

        return services;
    }
}
