using Articles.Abstractions.Enums;

namespace Articles.IntegrationEvents.Contracts;

public record class AssetDto(
    int Id,
    string Name,
    AssetType Type,
    FileDto File
    );

public record class FileDto(
    string OriginalName,
    string Name,
    string Extension,
    string FileServerId,
    long Size
    );