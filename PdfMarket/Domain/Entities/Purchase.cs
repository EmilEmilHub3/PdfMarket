namespace PdfMarket.Domain.Entities;

public class Purchase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string PdfId { get; set; } = default!;
    public string BuyerUserId { get; set; } = default!;

    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Price paid in points at the time of purchase.
    /// </summary>
    public int PriceInPoints { get; set; }
}
