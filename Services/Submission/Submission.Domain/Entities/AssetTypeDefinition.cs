using Blocks.Core.Cache;
using Blocks.Domain.Entities;
using Submission.Domain.ValueObjects;

namespace Submission.Domain.Entities;

public class AssetTypeDefinition : EnumEntity<AssetType>, ICacheable
{
    public required byte MaxFileSizeInMB { get; init; }

    public int MaxFileSizeInBytes => MaxFileSizeInMB * 1024 * 1024;

    public required string DefaultFileExtension { get; init; } = default!;

    public required FileExtensions AllowedFileExtensions { get; init; }

    public int MaxAssetCount { get; init; } 

    public bool AllowsMultipleAssets => MaxAssetCount > 1;
}
