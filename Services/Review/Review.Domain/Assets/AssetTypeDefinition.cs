using Articles.Abstractions.Enums;
using Blocks.Domain.Entities;
using Review.Domain.Assets.ValueObjects;

namespace Review.Domain.Assets;

public class AssetTypeDefinition : EnumEntity<AssetType>
{
    public required FileExtensions AllowedFileExtensions { get; init; }
    public required string DeafultFileExtension { get; init; } = default!;
    public required byte MaxFileSizeInMB { get; init; }
    public required byte MaxAssetCount { get; init; }
}
