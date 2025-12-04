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
        var user = await userRepository.GetByIdAsync(buyerUserId);
        if (user is null)
            return null;

        var pdf = await pdfRepository.GetByIdAsync(request.PdfId);
        if (pdf is null || !pdf.IsActive)
            return null;

        if (user.PointsBalance < pdf.PriceInPoints)
            throw new InvalidOperationException("Not enough points");

        user.PointsBalance -= pdf.PriceInPoints;
        user.OwnedPdfIds.Add(pdf.Id);
        await userRepository.UpdateAsync(user);

        var purchase = new Purchase
        {
            BuyerUserId = user.Id,
            PdfId = pdf.Id,
            PriceInPoints = pdf.PriceInPoints
        };

        await purchaseRepository.AddAsync(purchase);

        return new PurchaseResponse(
            purchase.Id,
            purchase.PdfId,
            purchase.BuyerUserId,
            purchase.PurchasedAt,
            purchase.PriceInPoints
        );
    }
}
