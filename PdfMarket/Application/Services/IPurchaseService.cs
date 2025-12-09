// src/Application/Services/IPurchaseService.cs
using PdfMarket.Contracts.Purchases;

namespace PdfMarket.Application.Services;

public interface IPurchaseService
{
    Task<PurchaseResponse?> PurchaseAsync(string buyerUserId, PurchaseRequest request);

    /// <summary>
    /// Returns all purchases for a given user, enriched with PDF title etc.
    /// Used by GET /api/purchases/my.
    /// </summary>
    Task<IReadOnlyCollection<PurchasedPdfDto>> GetMyPurchasesAsync(string buyerUserId);

    /// <summary>
    /// Returns true if the user has bought the given PDF.
    /// (Can be used to protect downloads.)
    /// </summary>
    Task<bool> HasUserPurchasedPdfAsync(string buyerUserId, string pdfId);
}
