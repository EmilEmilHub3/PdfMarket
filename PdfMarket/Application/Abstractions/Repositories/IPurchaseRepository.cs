using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Abstractions.Repositories;

/// <summary>
/// Repository abstraction for purchase records.
/// </summary>
public interface IPurchaseRepository
{
    /// <summary>
    /// Inserts a new purchase record.
    /// </summary>
    Task AddAsync(Purchase purchase);

    /// <summary>
    /// Returns all purchases made by a specific user.
    /// </summary>
    Task<IReadOnlyCollection<Purchase>> GetByBuyerAsync(string buyerUserId);

    /// <summary>
    /// Returns all purchases in the system.
    /// Used by admin statistics.
    /// </summary>
    Task<IReadOnlyCollection<Purchase>> GetAllAsync();
}
