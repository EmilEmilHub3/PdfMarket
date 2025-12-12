namespace PdfMarket.Domain.Entities;

/// <summary>
/// Domain entity representing a purchase transaction of a PDF.
/// </summary>
public class Purchase
{
    /// <summary>
    /// Unique identifier for this purchase record.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Identifier of the purchased PDF.
    /// </summary>
    public string PdfId { get; set; } = default!;

    /// <summary>
    /// Identifier of the user who made the purchase.
    /// </summary>
    public string BuyerUserId { get; set; } = default!;

    /// <summary>
    /// UTC timestamp for when the purchase occurred.
    /// </summary>
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Price paid in points at the time of purchase.
    /// Stored to preserve historical accuracy if PDF price changes later.
    /// </summary>
    public int PriceInPoints { get; set; }
}
