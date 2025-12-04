using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using PdfMarket.Application.Abstractions.Storage;

namespace PdfMarket.Infrastructure.Mongo;

public class GridFsFileStorage : IFileStorage
{
    private readonly IGridFSBucket bucket;

    public GridFsFileStorage(IMongoDatabase database)
    {
        bucket = new GridFSBucket(database, new GridFSBucketOptions
        {
            BucketName = "pdfFiles" // creates pdfFiles.files + pdfFiles.chunks collections
        });
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null)
    {
        var options = new GridFSUploadOptions();

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            options.Metadata = new BsonDocument
            {
                { "contentType", contentType }
            };
        }

        var id = await bucket.UploadFromStreamAsync(fileName, fileStream, options);
        return id.ToString(); // store as string on PdfDocument.FileStorageId
    }

    public async Task DownloadAsync(string storageId, Stream target)
    {
        var objectId = ObjectId.Parse(storageId);
        await bucket.DownloadToStreamAsync(objectId, target);
    }

    public async Task DeleteAsync(string storageId)
    {
        var objectId = ObjectId.Parse(storageId);
        await bucket.DeleteAsync(objectId);
    }
}
