namespace PdfMarket.Application.Abstractions.Storage;

/// <summary>
/// Abstraction for file storage.
/// Implementations may use GridFS, cloud storage, local disk, etc.
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Uploads a file stream and returns a storage identifier
    /// (e.g., GridFS ObjectId represented as a string).
    /// </summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null);

    /// <summary>
    /// Downloads a stored file into the provided target stream.
    /// </summary>
    Task DownloadAsync(string storageId, Stream target);

    /// <summary>
    /// Deletes a stored file.
    /// Used for cleanup when PDFs are removed by admins.
    /// </summary>
    Task DeleteAsync(string storageId);
}
