using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using PdfMarket.Application.Abstractions.Storage;

namespace PdfMarket.Infrastructure.Mongo;

/// <summary>
/// GridFS-based file storage implementation.
/// Responsible for storing, retrieving, and deleting PDF files in MongoDB.
/// </summary>
public class GridFsFileStorage : IFileStorage
{
    private readonly IGridFSBucket bucket;

    /// <summary>
    /// Initializes a new GridFS bucket for PDF file storage.
    /// </summary>
    public GridFsFileStorage(IMongoDatabase database)
    {
        bucket = new GridFSBucket(database, new GridFSBucketOptions
        {
            // Creates collections: pdfFiles.files and pdfFiles.chunks
            BucketName = "pdfFiles"
        });
    }

    /// <summary>
    /// Uploads a file stream to GridFS and returns its storage identifier.
    /// </summary>
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null)
    {
        var options = new GridFSUploadOptions();

        // Store content type as metadata for later download responses.
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            options.Metadata = new BsonDocument
            {
                { "contentType", contentType }
            };
        }

        var id = await bucket.UploadFromStreamAsync(fileName, fileStream, options);

        // Stored as string to keep domain entities storage-agnostic.
        return id.ToString();
    }

    /// <summary>
    /// Downloads a file from GridFS into the provided target stream.
    /// </summary>
    public async Task DownloadAsync(string storageId, Stream target)
    {
        var objectId = ObjectId.Parse(storageId);
        await bucket.DownloadToStreamAsync(objectId, target);
    }

    /// <summary>
    /// Deletes a file from GridFS.
    /// </summary>
    public async Task DeleteAsync(string storageId)
    {
        var objectId = ObjectId.Parse(storageId);
        await bucket.DeleteAsync(objectId);
    }
}
