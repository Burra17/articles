using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArticleHub.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ArticleHubDbContext>(options
            => options.UseNpgsql(configuration.GetConnectionString("Database")));

        var hasuraOptions = configuration.GetSectionByTypeName<HasuraOptions>();

        services.AddSingleton(_ =>
        {
            var grapgQLClientOptions = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(hasuraOptions.Endpoint)
            };

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            var graphQLHttpClient = new GraphQLHttpClient(grapgQLClientOptions, new SystemTextJsonSerializer(jsonSerializerOptions));

            graphQLHttpClient.HttpClient.DefaultRequestHeaders.Add("x-hasura-admin-secret", hasuraOptions.AdminSecret);

            return graphQLHttpClient;
        });

        services.AddScoped<ArticleGraphQLReadStore>();

        return services;
    }
}
