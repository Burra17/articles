using Articles.Abstractions.Events.Dtos;
using Review.Domain.Assets;

namespace Review.Domain.Articles;

public partial class Article
{
    public static Article AcceptSubmitted(ArticleDto articleDto, IEnumerable<ArticleActor> actors, IEnumerable<Asset> assets)
    {
        var article = new Article
        {
            Id = articleDto.Id,
            JournalId = articleDto.Journal.Id,
            Title = articleDto.Title,
            Type = articleDto.Type,
            Scope = articleDto.Scope,
            SubmittedById = articleDto.SubmittedBy.Id,
            SubmittedOn = articleDto.SubmittedOn,
            Stage = articleDto.Stage,
        };
        article._actors.AddRange(actors);
        article._assets.AddRange(assets);

        return article;
    }
}
