using Articles.Abstractions.Enums;
using Blocks.Domain.Entities;

namespace Submission.Domain.Entities;

public partial class Journal : IEntity
{
    public Article CreateArticle(string title, ArticleType Type, string scope)
    {
        var article = new Article()
        {
            Title = title,
            Type = Type,
            Scope = scope,
            Journal = this,
            Stage = ArticleStage.Created,
        };
        _articles.Add(article);
        // todo - add a domain event here later
        return article;
    }
}
