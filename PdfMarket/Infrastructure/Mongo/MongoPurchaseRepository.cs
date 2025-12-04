using MongoDB.Driver;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.Mongo;

public class MongoPurchaseRepository : IPurchaseRepository
{
    private readonly IMongoCollection<Purchase> purchases;

    public MongoPurchaseRepository(IMongoDatabase database)
    {
        purchases = database.GetCollection<Purchase>("purchases");
    }

    public async Task AddAsync(Purchase purchase)
    {
        await purchases.InsertOneAsync(purchase);
    }

    public async Task<IReadOnlyCollection<Purchase>> GetByBuyerAsync(string buyerUserId)
    {
        var list = await purchases
            .Find(p => p.BuyerUserId == buyerUserId)
            .ToListAsync();

        return list;
    }

    public async Task<IReadOnlyCollection<Purchase>> GetAllAsync()
    {
        var list = await purchases
            .Find(_ => true)
            .ToListAsync();

        return list;
    }
}
