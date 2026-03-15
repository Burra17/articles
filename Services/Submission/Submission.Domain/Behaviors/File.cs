using FileStorage.Contracts;
using Submission.Domain.Entities;

namespace Submission.Domain.ValueObjects;

public partial class File
{
    private File() { }

    internal static File CreateFile(FileMetadata fileMetadata, Asset asset, AssetTypeDefinition assetType)
    {
        var fileName = System.IO.Path.GetFileName(fileMetadata.StoragePath);
        var extension = FileExtension.FromFileName(fileName, assetType);
        var file = new File()
        {
            Name = FileName.FromAsset(asset, extension),
            Extension = extension,
            OriginalName = fileName,
            Size = fileMetadata.FileSize,
            FileServerId = fileMetadata.FileId,
        };

        return file;
    }
}
