namespace PdfMarket.Application.Abstractions.Storage;

public interface IFileStorage
{
    /// <summary>
    /// Uploads a file stream and returns a storage id (e.g. GridFS ObjectId as string).
    /// </summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null);

    /// <summary>
    /// Downloads a file with the given storage id into the provided target stream.
    /// </summary>
    Task DownloadAsync(string storageId, Stream target);

    /// <summary>
    /// Deletes a stored file (optional, but useful for cleanup).
    /// </summary>
    Task DeleteAsync(string storageId);
}
