// src/Application/Services/PurchaseService.cs
using PdfMarket.Contracts.Purchases;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Services;

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
        // 1) Find buyer
        var buyer = await userRepository.GetByIdAsync(buyerUserId);
        if (buyer is null)
            return null;

        // 2) Find pdf
        var pdf = await pdfRepository.GetByIdAsync(request.PdfId);
        if (pdf is null || !pdf.IsActive)
            return null;

        // 3) Find seller (uploader)
        var seller = await userRepository.GetByIdAsync(pdf.UploaderUserId);
        if (seller is null)
            throw new InvalidOperationException("Uploader not found");

        // 4) Check buyer has enough points
        if (buyer.PointsBalance < pdf.PriceInPoints)
            throw new InvalidOperationException("Not enough points");

        // 5) Move points: buyer -> seller
        buyer.PointsBalance -= pdf.PriceInPoints;
        buyer.OwnedPdfIds.Add(pdf.Id);

        // If buyer and seller are different users, credit seller
        if (seller.Id != buyer.Id)
        {
            seller.PointsBalance += pdf.PriceInPoints;
        }
        // If they are the same user (buying own PDF), net effect is:
        // - buyer loses points
        // - no extra credit
        // You can change this later if you want a different rule.

        // 6) Persist users
        await userRepository.UpdateAsync(buyer);
        if (seller.Id != buyer.Id)
        {
            await userRepository.UpdateAsync(seller);
        }

        // 7) Create purchase record
        var purchase = new Purchase
        {
            BuyerUserId = buyer.Id,
            PdfId = pdf.Id,
            PriceInPoints = pdf.PriceInPoints
        };

        await purchaseRepository.AddAsync(purchase);

        // 8) Return response
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
        var purchases = await purchaseRepository.GetByBuyerAsync(buyerUserId);
        return purchases.Any(p => p.PdfId == pdfId);
    }
}
