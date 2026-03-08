using Auth.Grpc;
using Blocks.AspNetCore.Grpc;
using Blocks.Core;
using FileStorage.MongoGridFS;
using Journals.Grpc;

namespace Submission.API;

public static class DependencyInjection
{
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
