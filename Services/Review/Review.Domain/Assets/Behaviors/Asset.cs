using Articles.IntegrationEvents.Contracts;
using FileStorage.Contracts;
using Review.Domain.Assets.Enums;
using Review.Domain.Assets.ValueObjects;
using File = Review.Domain.Assets.ValueObjects.File;

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

        return asset;
    }

    public File CreateFile(FileMetadata fileMetadata, AssetTypeDefinition assetType)
    {
        File = File.CreateFile(fileMetadata, this, assetType);

        State = AssetState.Uploaded;

        return File;
    }
}
