using Blocks.Domain.ValueObjects;

namespace Review.Domain.Assets.ValueObjects;

public class FileName : StringValueObject
{
    internal FileName(string value) => Value = value;

    public static FileName From(Asset asset, FileExtensions extension)
    {
        var assetName = asset.Name.Value.Replace("'", "").Replace(" ", "-");
        return new FileName($"{assetName}.{extension}");
    }
}
