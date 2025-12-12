using MongoDB.Driver;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Contracts.Pdfs;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.Mongo;

/// <summary>
/// MongoDB repository for managing PDF documents.
/// </summary>
public class MongoPdfRepository : IPdfRepository
{
    private readonly IMongoCollection<PdfDocument> pdfs;

    /// <summary>
    /// Initializes the PDF collection.
    /// </summary>
    public MongoPdfRepository(IMongoDatabase database)
    {
        pdfs = database.GetCollection<PdfDocument>("pdfs");
    }

    /// <summary>
    /// Returns a PDF by its identifier.
    /// </summary>
    public async Task<PdfDocument?> GetByIdAsync(string id)
    {
        return await pdfs
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Returns publicly visible PDFs matching the provided filter.
    /// Only active PDFs are included.
    /// </summary>
    public async Task<IReadOnlyCollection<PdfDocument>> BrowseAsync(PdfFilterRequest filter)
    {
        var builder = Builders<PdfDocument>.Filter;
        var filters = new List<FilterDefinition<PdfDocument>>
        {
            builder.Eq(p => p.IsActive, true)
        };

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            filters.Add(builder.Or(
                builder.Regex(p => p.Title, filter.Query),
                builder.Regex(p => p.Description, filter.Query)
            ));
        }

        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            filters.Add(builder.AnyEq(p => p.Tags, filter.Tag));
        }

        if (filter.MinPriceInPoints.HasValue)
        {
            filters.Add(builder.Gte(p => p.PriceInPoints, filter.MinPriceInPoints.Value));
        }

        if (filter.MaxPriceInPoints.HasValue)
        {
            filters.Add(builder.Lte(p => p.PriceInPoints, filter.MaxPriceInPoints.Value));
        }

        var combined = builder.And(filters);

        return await pdfs
            .Find(combined)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Returns all active PDFs uploaded by a specific user.
    /// </summary>
    public async Task<IReadOnlyCollection<PdfDocument>> GetByUploaderAsync(string uploaderUserId)
    {
        return await pdfs
            .Find(p => p.UploaderUserId == uploaderUserId && p.IsActive)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Returns all PDFs uploaded by a specific user, including inactive ones.
    /// Used for "My uploads" management.
    /// </summary>
    public async Task<IReadOnlyCollection<PdfDocument>> GetAllByUploaderAsync(string uploaderUserId)
    {
        return await pdfs
            .Find(p => p.UploaderUserId == uploaderUserId)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Inserts a new PDF document.
    /// </summary>
    public async Task AddAsync(PdfDocument pdf)
    {
        await pdfs.InsertOneAsync(pdf);
    }

    /// <summary>
    /// Updates an existing PDF document.
    /// </summary>
    public async Task UpdateAsync(PdfDocument pdf)
    {
        await pdfs.ReplaceOneAsync(
            p => p.Id == pdf.Id,
            pdf,
            new ReplaceOptions { IsUpsert = false });
    }

    /// <summary>
    /// Returns all PDFs in the system (active and inactive).
    /// </summary>
    public async Task<IReadOnlyCollection<PdfDocument>> GetAllAsync()
    {
        return await pdfs
            .Find(FilterDefinition<PdfDocument>.Empty)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Deletes a PDF document by identifier.
    /// </summary>
    public async Task DeleteAsync(string id)
    {
        await pdfs.DeleteOneAsync(p => p.Id == id);
    }
}
