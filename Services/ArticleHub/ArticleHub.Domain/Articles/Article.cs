using Articles.Abstractions.Enums;
using Blocks.Domain.Entities;

namespace ArticleHub.Domain.Articles;

public class Article : AggregateRoot
{
    public required string Title { get; set; }
    
    public string? Doi { get; set; }

    public ArticleStage Stage { get; set; }

    public virtual int SubmittedById { get; set; }
    public virtual Person SubmittedBy { get; set; } = null!;

    public DateTime SubmittedOn { get; set; }
    public DateTime? AcceptedOn { get; set; }
    public DateTime? PublishedOn { get; set; }

    public required int JournalId { get; set; }
    public Journal Journal { get; set; } = null!;

    public List<ArticleActor> Actors { get; set; } = new();
}
