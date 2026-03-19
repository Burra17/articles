using ArticleHub.Persistence;
using Articles.Security;
using Blocks.Core;
using Blocks.Mapster;
using Blocks.Messaging;
using Blocks.Messaging.MassTransit;
using Carter;
using Microsoft.AspNetCore.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ArticleHub.API;

public static class DependencyInjection
{
    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAndValidateOptions<RabbitMqOptions>(configuration)
            .AddAndValidateOptions<HasuraOptions>(configuration)
            .Configure<JsonOptions>(opt =>
            {
                opt.SerializerOptions.PropertyNameCaseInsensitive = true;
                opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }

    public static IServiceCollection AddApiAndApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCarter()
            .AddHttpContextAccessor()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddJwtAuthentication(configuration)
            .AddAuthorization();

        services
            .AddMemoryCache()
            .AddMapsterConfigsFromCurrentAssembly()
            .AddMassTransitWithRabbitMQ(configuration, Assembly.GetExecutingAssembly());

        return services;
    }
}
