using Articles.Abstractions.Enums;
using Blocks.Domain.Entities;

namespace Submission.Domain.Entities
{
    public partial class Article : AggregateRoot
    {
        public required string Title { get; set; }
        public required string Scope { get; set; }
        public required ArticleType Type { get; set; }
        public ArticleStage Stage { get; internal set; }
        public int JournalId { get; init; }
        public required Journal Journal { get; init; } 

        public int? SubmittedById { get; set; }
        public Person? SubmittedBy { get; set; }

        private readonly List<Asset> _assets = new();
        public IReadOnlyList<Asset> Assets => _assets.AsReadOnly();

        public List<ArticleActor> Actors { get; init; } = new();
    }
}
