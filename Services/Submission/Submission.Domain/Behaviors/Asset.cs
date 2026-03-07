using FileStorage.Contracts;

namespace Submission.Domain.Entities;

public partial class Asset
{
    private Asset() { }

    internal static Asset Create(Article article, AssetTypeDefinition type)
    {
        return new Asset
        {
            ArticleId = article.Id,
            Article = article,
            Name = AssetName.FromAssetType(type),
            Type = type.Name
        };
    }

    public string GenerateStorageFilePath(string fileName)
        => $"Articles/{Article.Id}/{Name}/{fileName}";

    public File CreateFile(UploadResponse uploadResponse, AssetTypeDefinition assetType)
    {
        File = File.CreateFile(uploadResponse, this, assetType);

        return File;
    }
}
