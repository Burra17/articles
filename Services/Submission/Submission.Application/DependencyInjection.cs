using Blocks.Mapster;
using Blocks.MediatR.Behaviours;
using Blocks.Messaging.MassTransit;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Submission.Application.Features.CreateArticle;
using System.Reflection;

namespace Submission.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddMapsterConfigsFromCurrentAssembly()
            .AddValidatorsFromAssemblyContaining<CreateArticleCommandValidator>() // Register FluentValidation validators as transient services
            .AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(AssignUserIdBehavior<,>));
            })
            .AddMassTransitWithRabbitMQ(config, Assembly.GetExecutingAssembly());

        return services;
    }
}
