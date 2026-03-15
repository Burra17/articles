using FileStorage.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace FileStorage.MongoGridFS;

public class FileService(GridFSBucket bucket, IOptions<MongoGridFsFileStorageOptions> options)
    : FileService<MongoGridFsFileStorageOptions>(bucket, options);

public class FileService<TFileStorageOptions> : IFileService<TFileStorageOptions>
    where TFileStorageOptions :MongoGridFsFileStorageOptions
{
    private readonly GridFSBucket _bucket;
    private readonly MongoGridFsFileStorageOptions _options;

    private const string FilePathMetadataKey = "filePath";
    private const string ContentTypeMetadataKey = "contentType";
    private const string DefaultContentType = "application/octet-stream";

    public FileService(GridFSBucket bucket, IOptions<TFileStorageOptions> options)
        => (_bucket, _options) = (bucket, options.Value);

    public async Task<FileMetadata> UploadFileAsync
        (string storagePath, IFormFile file, bool overwrite = false, Dictionary<string, string>? tags = null, CancellationToken ct = default)
    {
        var request = new FileUploadRequest(storagePath, file.FileName, file.ContentType, file.Length);

        using var stream = file.OpenReadStream();

        return await UploadInternalAsync(request, stream, overwrite, tags, ct);
    }

    public async Task<FileMetadata> UploadFileAsync
        (FileUploadRequest request, Stream stream, bool overwrite = false, Dictionary<string, string>? tags = null, CancellationToken ct = default)
    {
        request = request with { FileSize = stream.Length };

        return await UploadInternalAsync(request, stream, overwrite,tags, ct);
    }

    private async Task<FileMetadata> UploadInternalAsync
        (FileUploadRequest request, Stream stream, bool overwrite = false, Dictionary<string, string>? tags = null, CancellationToken ct = default)
    {
        if (request.FileSize > _options.FileSizeLimitInBytes)
            throw new InvalidOperationException($"File exceeds maximum allowed size of {_options.FileSizeLimitInMB} MB.");

        if (overwrite)
            await TryDeleteFileAsync(request.StoragePath, ct);

        var metadata = new BsonDocument(tags ?? new Dictionary<string, string>())
        {
            { nameof(FileMetadata.StoragePath), request.StoragePath },
            { nameof(FileMetadata.FileName), request.FileName },
            { nameof(FileMetadata.ContentType), request.ContentType ?? DefaultContentType } 
        };

        var uploadOptions = new GridFSUploadOptions
        {
            Metadata = metadata,
            ChunkSizeBytes = _options.ChunkSizeBytes
        };

        Object fileId;
        fileId = await _bucket.UploadFromStreamAsync(request.FileName, stream, uploadOptions, ct);

        return new FileMetadata(
            StoragePath: request.StoragePath,
            FileName: request.FileName,
            ContentType: request.ContentType ?? DefaultContentType,
            FileSize: request.FileSize,
            FileId: fileId.ToString()
        );
    }

    public async Task<(Stream FileStream, FileMetadata FileMetaData)> DownloadFileAsync(string fileId, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(fileId, out var id))
            throw new FileNotFoundException($"Invalid file ID: {fileId}");

        var fileInfo = await _bucket.Find(Builders<GridFSFileInfo>.Filter.Eq("_id", fileId)).FirstOrDefaultAsync();
        if (fileInfo == null)
            throw new FileNotFoundException($"No file was found with ID: {fileId}");

        var stream = await _bucket.OpenDownloadStreamAsync(fileId);

        var metadata = fileInfo.Metadata;
        var fileMetadata = new FileMetadata(
            StoragePath: metadata[nameof(FileMetadata.StoragePath)].AsString,
            FileName: metadata[nameof(FileMetadata.FileName)].AsString,
            ContentType: metadata[nameof(FileMetadata.ContentType)].AsString,
            FileSize: fileInfo.Length,
            FileId: fileId.ToString()
            );

        return (stream, fileMetadata);
    }

    public async Task<bool> TryDeleteFileAsync(string fileId, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(fileId, out var objectId))
            return false;

        try
        {
            await _bucket.DeleteAsync(objectId, ct);
                return true;
        }
        catch (GridFSFileNotFoundException) { return false; }
    }
}
