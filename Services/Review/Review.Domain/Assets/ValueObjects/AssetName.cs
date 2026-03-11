using Blocks.Domain.ValueObjects;

namespace Review.Domain.Assets.ValueObjects;

public class AssetName : StringValueObject
{
    private AssetName(string value) => Value = value;

    public static AssetName FromAssetType(AssetTypeDefiniton assetType)
        => new AssetName(assetType.Name.ToString());
}
