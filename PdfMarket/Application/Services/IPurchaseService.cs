using PdfMarket.Contracts.Purchases;

namespace PdfMarket.Application.Services;

public interface IPurchaseService
{
    Task<PurchaseResponse?> PurchaseAsync(string buyerUserId, PurchaseRequest request);
}
