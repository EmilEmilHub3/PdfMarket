using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.InMemory;

public class InMemoryPurchaseRepository : IPurchaseRepository
{
    private readonly List<Purchase> purchases = new();

    public Task AddAsync(Purchase purchase)
    {
        purchases.Add(purchase);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Purchase>> GetByBuyerAsync(string buyerUserId)
    {
        var result = purchases
            .Where(p => p.BuyerUserId == buyerUserId)
            .ToList();

        return Task.FromResult((IReadOnlyCollection<Purchase>)result);
    }

    public Task<IReadOnlyCollection<Purchase>> GetAllAsync()
    {
        return Task.FromResult((IReadOnlyCollection<Purchase>)purchases.ToList());
    }
}
