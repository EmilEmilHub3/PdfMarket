using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Contracts.Pdfs;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.InMemory;

public class InMemoryPdfRepository : IPdfRepository
{
    private readonly List<PdfDocument> pdfs = new();

    public Task<PdfDocument?> GetByIdAsync(string id) =>
        Task.FromResult(pdfs.FirstOrDefault(p => p.Id == id));

    public Task<IReadOnlyCollection<PdfDocument>> BrowseAsync(PdfFilterRequest filter)
    {
        IEnumerable<PdfDocument> query = pdfs.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            query = query.Where(p =>
                p.Title.Contains(filter.Query, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(filter.Query, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            query = query.Where(p => p.Tags.Contains(filter.Tag));
        }

        if (filter.MinPriceInPoints.HasValue)
        {
            query = query.Where(p => p.PriceInPoints >= filter.MinPriceInPoints.Value);
        }

        if (filter.MaxPriceInPoints.HasValue)
        {
            query = query.Where(p => p.PriceInPoints <= filter.MaxPriceInPoints.Value);
        }

        return Task.FromResult((IReadOnlyCollection<PdfDocument>)query.ToList());
    }

    public Task AddAsync(PdfDocument pdf)
    {
        pdfs.Add(pdf);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PdfDocument pdf)
    {
        // In-memory: object already in list, nothing special required
        return Task.CompletedTask;
    }
}
