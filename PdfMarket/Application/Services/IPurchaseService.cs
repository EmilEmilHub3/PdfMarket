using PdfMarket.Contracts.Purchases;

namespace PdfMarket.Application.Services;

/// <summary>
/// Application service for purchasing logic and purchase history.
/// </summary>
public interface IPurchaseService
{
    /// <summary>
    /// Purchases a PDF on behalf of the given buyer.
    /// Returns null if buyer or PDF is not found (or PDF is inactive).
    /// Throws if business rules fail (e.g., not enough points).
    /// </summary>
    Task<PurchaseResponse?> PurchaseAsync(string buyerUserId, PurchaseRequest request);

    /// <summary>
    /// Returns all purchases for a user, enriched with PDF title, etc.
    /// Used by GET /api/purchases/my.
    /// </summary>
    Task<IReadOnlyCollection<PurchasedPdfDto>> GetMyPurchasesAsync(string buyerUserId);

    /// <summary>
    /// Returns true if the user has bought the given PDF.
    /// Can be used to protect downloads.
    /// </summary>
    Task<bool> HasUserPurchasedPdfAsync(string buyerUserId, string pdfId);
}
