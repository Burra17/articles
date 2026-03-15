using Blocks.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Review.Persistence.Repositories;

namespace Review.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ReviewDbContext>((provider, options) =>
        {
            options.AddInterceptors(provider.GetServices<ISaveChangesInterceptor>());
        });

        services.AddScoped(typeof(Repository<>));
        services.AddConcreteImplementationsOfGeneric(typeof(Repository<>)); // inherits from Repository
        services.AddScoped<AssetTypeDefinitionRepository>();

        return services;
    }
}
