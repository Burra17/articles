using ArticleHub.Domain.Dtos;
using Blocks.Core.GraphQL;
using GraphQL;
using GraphQL.Client.Http;

namespace ArticleHub.Persistence;

public class ArticleGraphQLReadStore(GraphQLHttpClient _graphQLHttpClient)
{
    public async Task<QueryResult<ArticleDto>> GetArticlesAsync(object filter, int limit = 20, int offset = 0, CancellationToken ct = default)
    {
        var request = new GraphQLRequest
        {


            Query = @"
                query GetArticles(@filter: article_bool_exp){
                    items:article(where: $filter) {
                        id
                        title
                        doi
                        stage
                        submittedon
                        publishedon
                        acceptedon
                        journal:journal {
                            abbreviation
                            name
                        }
                        submittedby:person {
                            email
                            firstname
                            lastname
                        }
                }",

            Variables = new { filter, limit, offset }
        };

        var response = await _graphQLHttpClient.SendQueryAsync<QueryResult<ArticleDto>>(request, ct);

        return response.Data;
    }
}
