using Blocks.Domain.ValueObjects;

namespace Review.Domain.Assets.ValueObjects;

public class FileExtension : StringValueObject
{
    private FileExtension(string value) => Value = value;

    public static FileExtension FromAssetType(AssetTypeDefinition assetType)
    {
        return new FileExtension(assetType.DeafultFileExtension);
    }

    public static FileExtension FromFileName(string fileName, AssetTypeDefinition assetType)
    {
        var extension = Path.GetExtension(fileName).Remove(0, 1);
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);
        ArgumentOutOfRangeException.ThrowIfNotEqual(
            assetType.AllowedFileExtensions.IsValidExtension(extension), true);

        return new FileExtension(extension);
    }
}
