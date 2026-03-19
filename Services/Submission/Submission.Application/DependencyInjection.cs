using Blocks.Mapster;
using Blocks.MediatR.Behaviours;
using Blocks.Messaging.MassTransit;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Submission.Application.Features.CreateArticle;
using Microsoft.Extensions.Caching.Memory;
using Submission.Application.StateMachines;
using Submission.Domain.StateMachines;
using System.Reflection;

namespace Submission.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddMapsterConfigsFromCurrentAssembly()
            .AddValidatorsFromAssemblyContaining<CreateArticleCommandValidator>()
            .AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(AssignUserIdBehavior<,>));
            })
            .AddMassTransitWithRabbitMQ(config, Assembly.GetExecutingAssembly());

        services.AddScoped<ArticleStateMachineFactory>(provider => articleStage =>
        {
            var cache = provider.GetRequiredService<IMemoryCache>();
            return new ArticleStateMachine(articleStage, cache);
        });

        return services;
    }
}
