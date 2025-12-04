using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Abstractions.Repositories;

public interface IPurchaseRepository
{
    Task AddAsync(Purchase purchase);
    Task<IReadOnlyCollection<Purchase>> GetByBuyerAsync(string buyerUserId);
    Task<IReadOnlyCollection<Purchase>> GetAllAsync();
}
