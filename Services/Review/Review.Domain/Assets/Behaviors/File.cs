using Articles.Abstractions.Events.Dtos;

namespace Review.Domain.Assets.ValueObjects;

public partial class File
{
    public static File CreateFromSubmission(FileDto fileDto, AssetTypeDefinition assetType)
    {
        var extension = FileExtension.FromFileName(fileDto.OriginalName, assetType);

        var file = new File()
        {
            Name = new FileName(fileDto.Name),
            Extension = extension,
            OriginalName = fileDto.OriginalName,
            Size = fileDto.Size,
            FileServerId = fileDto.FileServerId
        };

        return file;
    }
}
