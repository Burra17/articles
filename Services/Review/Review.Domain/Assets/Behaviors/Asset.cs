using Articles.Abstractions.Events.Dtos;
using Review.Domain.Assets.Enums;
using Review.Domain.Assets.ValueObjects;

namespace Review.Domain.Assets;

public partial class Asset
{
    public static Asset CreateFromSubmission(AssetDto assetDto, AssetTypeDefinition type, int articleId)
    {
        var asset = new Asset()
        {
            ArticleId = articleId,
            Name = AssetName.FromAssetType(type),
            Type = type.Id,
            State = AssetState.Uploaded,
        };

        asset.File = ValueObjects.File.CreateFromSubmission(assetDto.File, type);

        return asset;
    }
}
