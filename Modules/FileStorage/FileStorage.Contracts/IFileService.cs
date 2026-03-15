using Microsoft.AspNetCore.Http;

namespace FileStorage.Contracts;

public interface IFileService<TFileStorageOptions> : IFileService
    where TFileStorageOptions : IFileStorageOptions;


public interface IFileService
{
    Task<FileMetadata> UploadFileAsync(string filepath, IFormFile file, bool overwrite = false, Dictionary<string, string>? tags = null, CancellationToken ct = default);
    Task<FileMetadata> UploadFileAsync(FileUploadRequest request, Stream stream, bool overwrite = false, Dictionary<string, string>? tags = null, CancellationToken ct = default);

    Task<(Stream FileStream, FileMetadata FileMetaData)> DownloadFileAsync(string fileId, CancellationToken ct = default);

    Task<bool> TryDeleteFileAsync(string FileId, CancellationToken ct = default);
}

public interface IFileStorageOptions;

public record FileUploadRequest(string StoragePath, string FileName, string ContentType, long FileSize = default);

public record FileMetadata(string StoragePath, string FileName, string ContentType, long FileSize, string FileId);
