using Blocks.Domain.Entities;

namespace ArticleHub.Domain.Articles;

public class Journal : Entity
{
    public required string Abbreviation { get; init; }
    public required string Name { get; init; }

    public virtual ICollection<Article> Articles { get; } = new List<Article>();
}
