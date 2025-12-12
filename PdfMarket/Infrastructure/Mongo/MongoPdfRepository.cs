using MongoDB.Driver;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Contracts.Pdfs;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.Mongo;

public class MongoPdfRepository : IPdfRepository
{
    private readonly IMongoCollection<PdfDocument> pdfs;

    public MongoPdfRepository(IMongoDatabase database)
    {
        pdfs = database.GetCollection<PdfDocument>("pdfs");
    }

    public async Task<PdfDocument?> GetByIdAsync(string id)
    {
        return await pdfs
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

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

        // ⭐ SORT HERE (newest → oldest)
        var list = await pdfs
            .Find(combined)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();

        return list;
    }


    public async Task AddAsync(PdfDocument pdf)
    {
        await pdfs.InsertOneAsync(pdf);
    }

    public async Task UpdateAsync(PdfDocument pdf)
    {
        await pdfs.ReplaceOneAsync(
            filter: p => p.Id == pdf.Id,
            replacement: pdf,
            options: new ReplaceOptions { IsUpsert = false });
    }

    public async Task<IReadOnlyCollection<PdfDocument>> GetAllAsync()
    {
        return await pdfs
            .Find(Builders<PdfDocument>.Filter.Empty)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task DeleteAsync(string id)
    {
        await pdfs.DeleteOneAsync(p => p.Id == id);
    }

}
