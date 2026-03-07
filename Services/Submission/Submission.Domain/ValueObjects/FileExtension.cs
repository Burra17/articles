using Blocks.Core;
using Blocks.Domain.ValueObjects;

public class FileExtension : StringValueObject
{
    private FileExtension(string value) => Value = value;

    public static FileExtension FromFileName(string fileName, AssetTypeDefinition assetType)
    {
        var extension = Path.GetExtension(fileName).Remove(0, 1); // Remove the dot from the extension
        Guard.ThrowIfNullOrWhiteSpace(extension);
        Guard.ThrowIfNotEqual(
            assetType.AllowedFileExtensions.IsValidExstension(extension), true);

        // todo - validate exstension against AssetType

        return new FileExtension(extension);
    }
}
