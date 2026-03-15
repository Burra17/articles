using Blocks.Core;
using Carter;
using Review.API.FileStorage;
using Articles.Security;
using FileStorage.MongoGridFS;
using EmailService.Smtp;
using Blocks.AspNetCore.Grpc;
using Auth.Grpc;
using Review.Application.Options;
using Blocks.Messaging;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using EmailService.Contracts;

namespace Review.API;

public static class DependencyInjection
{
    public static void ConfiguraApiOptions(this IServiceCollection services,  IConfiguration configuration)
    {
        services
            .AddAndValidateOptions<AppUrlsOptions>(configuration)
            .AddAndValidateOptions<EmailOptions>(configuration)
            .AddAndValidateOptions<RabbitMqOptions>(configuration)
            .Configure<JsonOptions>(opt =>
            {
                opt.SerializerOptions.PropertyNameCaseInsensitive = true;
                opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddMemoryCache()
            .AddCarter()
            .AddHttpContextAccessor()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddJwtAuthentication(config)
            .AddAuthorization();

        services.AddMongoFileStorageAsSingletone(config);
        services.AddMongoFileStorageAsScoped<SubmissionFileStorageOptions>(config);
        services.AddFileServiceFactory();

        services.AddSmtpEmailService(config);

        var grpcOptions = config.GetSectionByTypeName<GrpcServicesOptions>();
        services.AddCodeFirstGrpcClient<IPersonService> (grpcOptions, "Person");

        return services;
    }
}
