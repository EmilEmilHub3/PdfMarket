namespace PdfMarket.Domain.Entities;

public class PdfDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;

    /// <summary>
    /// User.Id of the uploader.
    /// </summary>
    public string UploaderUserId { get; set; } = default!;

    /// <summary>
    /// Price to buy this PDF, in points (int).
    /// </summary>
    public int PriceInPoints { get; set; }

    public string[] Tags { get; set; } = Array.Empty<string>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this PDF is visible/for sale.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Reference to the stored file (e.g. Mongo GridFS id). Can be filled in later.
    /// </summary>
    public string? FileStorageId { get; set; }
}
