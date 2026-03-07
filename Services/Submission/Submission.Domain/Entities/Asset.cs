using Articles.Abstractions.Enums;
using Blocks.Domain.Entities;
using Submission.Domain.ValueObjects;

namespace Submission.Domain.Entities;

public partial class Asset : IEntity
{
    public int Id { get; init; }
    public AssetName Name { get; private set; } = null!;
    public AssetType Type { get; private set; }

    public int ArticleId { get; private set; }
    public Article Article { get; private set; } = null!;

    public File File { get; private set; } = null!;
}
