namespace Review.Domain.Assets.ValueObjects;

public partial class File
{
    public required string OriginalName { get; init; } = default!;
    public required string FileServerId { get; init; } = default!;
    public required long Size { get; init; }
    public required FileName Name { get; init; }
    public required FileExtensions Extension { get; init; } = default!;
}
