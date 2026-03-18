using ArticleHub.Domain.Dtos;
using ArticleHub.Persistence;
using Carter;

namespace ArticleHub.API.Articles.SearchArticles;

public class SearchArticlesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/articles/graphql", async (SearchArticlesQuery articlesQuery, ArticleGraphQLReadStore graphQLReadStore, CancellationToken ct) =>
        {
            var response = await graphQLReadStore.GetArticlesAsync(
                articlesQuery.Filter,
                articlesQuery.Pagination.Limit,
                articlesQuery.Pagination.Offset,
                ct);

            return Results.Json(response);
        })
        .RequireAuthorization()
        .WithName("GetArticles")
        .WithTags("Articles")
        .Produces<IEnumerable<ArticleDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
