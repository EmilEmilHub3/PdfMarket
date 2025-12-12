using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Contracts.Purchases;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Services;

/// <summary>
/// Implements purchase business rules:
/// - validates buyer and PDF
/// - checks points
/// - transfers points
/// - records the purchase
/// </summary>
public class PurchaseService : IPurchaseService
{
    private readonly IPdfRepository pdfRepository;
    private readonly IUserRepository userRepository;
    private readonly IPurchaseRepository purchaseRepository;

    public PurchaseService(
        IPdfRepository pdfRepository,
        IUserRepository userRepository,
        IPurchaseRepository purchaseRepository)
    {
        this.pdfRepository = pdfRepository;
        this.userRepository = userRepository;
        this.purchaseRepository = purchaseRepository;
    }

    public async Task<PurchaseResponse?> PurchaseAsync(string buyerUserId, PurchaseRequest request)
    {
        // Buyer must exist (prevents purchases with invalid JWT identities).
        var buyer = await userRepository.GetByIdAsync(buyerUserId);
        if (buyer is null)
            return null;

        // PDF must exist and be active (inactive PDFs are not purchasable).
        var pdf = await pdfRepository.GetByIdAsync(request.PdfId);
        if (pdf is null || !pdf.IsActive)
            return null;

        // Seller (uploader) must exist for points transfer.
        var seller = await userRepository.GetByIdAsync(pdf.UploaderUserId);
        if (seller is null)
            throw new InvalidOperationException("Uploader not found");

        // Enforce the points economy rule.
        if (buyer.PointsBalance < pdf.PriceInPoints)
            throw new InvalidOperationException("Not enough points");

        // Transfer points and record ownership (OwnedPdfIds is a fast check for UI use later).
        buyer.PointsBalance -= pdf.PriceInPoints;
        buyer.OwnedPdfIds.Add(pdf.Id);

        // Avoid double-credit if user buys their own PDF.
        if (seller.Id != buyer.Id)
        {
            seller.PointsBalance += pdf.PriceInPoints;
        }

        // Persist updated balances.
        await userRepository.UpdateAsync(buyer);
        if (seller.Id != buyer.Id)
        {
            await userRepository.UpdateAsync(seller);
        }

        // Record the purchase as an immutable event-like entity.
        var purchase = new Purchase
        {
            BuyerUserId = buyer.Id,
            PdfId = pdf.Id,
            PriceInPoints = pdf.PriceInPoints
        };

        await purchaseRepository.AddAsync(purchase);

        // Return current balance so the frontend can update immediately.
        return new PurchaseResponse(
            purchase.Id,
            purchase.PdfId,
            purchase.BuyerUserId,
            purchase.PurchasedAt,
            purchase.PriceInPoints,
            buyer.PointsBalance
        );
    }

    public async Task<IReadOnlyCollection<PurchasedPdfDto>> GetMyPurchasesAsync(string buyerUserId)
    {
        var purchases = await purchaseRepository.GetByBuyerAsync(buyerUserId);
        if (purchases.Count == 0)
            return Array.Empty<PurchasedPdfDto>();

        var result = new List<PurchasedPdfDto>(purchases.Count);

        foreach (var purchase in purchases)
        {
            // Enrich the purchase with title (PDF may be missing if deleted by admin).
            var pdf = await pdfRepository.GetByIdAsync(purchase.PdfId);
            var title = pdf?.Title ?? "Unknown";

            result.Add(new PurchasedPdfDto(
                purchase.PdfId,
                title,
                purchase.PriceInPoints,
                purchase.PurchasedAt
            ));
        }

        return result;
    }

    public async Task<bool> HasUserPurchasedPdfAsync(string buyerUserId, string pdfId)
    {
        // Simple authorization check: does a purchase record exist for (buyer, pdf)?
        var purchases = await purchaseRepository.GetByBuyerAsync(buyerUserId);
        return purchases.Any(p => p.PdfId == pdfId);
    }
}
