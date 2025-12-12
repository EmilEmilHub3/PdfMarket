using MongoDB.Driver;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.Mongo;

/// <summary>
/// MongoDB repository for purchase records.
/// </summary>
public class MongoPurchaseRepository : IPurchaseRepository
{
    private readonly IMongoCollection<Purchase> purchases;

    /// <summary>
    /// Initializes the purchase collection.
    /// </summary>
    public MongoPurchaseRepository(IMongoDatabase database)
    {
        purchases = database.GetCollection<Purchase>("purchases");
    }

    /// <summary>
    /// Inserts a new purchase record.
    /// </summary>
    public async Task AddAsync(Purchase purchase)
    {
        await purchases.InsertOneAsync(purchase);
    }

    /// <summary>
    /// Returns all purchases made by a specific user.
    /// </summary>
    public async Task<IReadOnlyCollection<Purchase>> GetByBuyerAsync(string buyerUserId)
    {
        return await purchases
            .Find(p => p.BuyerUserId == buyerUserId)
            .ToListAsync();
    }

    /// <summary>
    /// Returns all purchase records in the system.
    /// </summary>
    public async Task<IReadOnlyCollection<Purchase>> GetAllAsync()
    {
        return await purchases
            .Find(_ => true)
            .ToListAsync();
    }
}
