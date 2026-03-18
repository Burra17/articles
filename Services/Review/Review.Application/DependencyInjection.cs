using Blocks.Mapster;
using Blocks.MediatR.Behaviours;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Blocks.Messaging.MassTransit;
using System.Text.Json.Serialization;
using Blocks.Messaging;
using Microsoft.AspNetCore.Http.Json;

namespace Review.Application;

public static class DependencyInjection
{
    public static void ConfigureApiOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAndValidateOptions<RabbitMqOptions>(configuration)
            .Configure<JsonOptions>(opt =>
            {
                opt.SerializerOptions.PropertyNameCaseInsensitive = true;
                opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }


    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMapsterConfigsFromCurrentAssembly()

            //.AddValidatorsFromAssemblyContaining<CreateArticleCommandValidator>()
            .AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(AssignUserIdBehavior<,>));
            })
            .AddMassTransitWithRabbitMQ(configuration, Assembly.GetExecutingAssembly());

        return services;
    }
}
