using FileStorage.MongoGridFS;

namespace Submission.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMemoryCache()               // For caching purposes, if needed
            .AddEndpointsApiExplorer()      // For API endpoint discovery in Swagger
            .AddSwaggerGen()                // For generating Swagger documentation
            ;

        services.AddMongoFileStorage(configuration);
        return services;
    }
}
