namespace PdfMarket.Domain.Entities;

/// <summary>
/// Domain entity representing an uploaded PDF and its metadata.
/// </summary>
public class PdfDocument
{
    /// <summary>
    /// Unique identifier for the PDF document.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Title shown in browse lists and PDF details.
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// Description shown on the PDF details page.
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// User.Id of the uploader/creator.
    /// </summary>
    public string UploaderUserId { get; set; } = default!;

    /// <summary>
    /// Price to buy this PDF in points.
    /// </summary>
    public int PriceInPoints { get; set; }

    /// <summary>
    /// Tags used for searching and filtering.
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Timestamp for when the PDF was created/uploaded.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indicates whether the PDF is publicly visible and purchasable.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Reference to the stored file (e.g., MongoDB GridFS ObjectId as string).
    /// Null means file not uploaded/linked yet.
    /// </summary>
    public string? FileStorageId { get; set; }
}
