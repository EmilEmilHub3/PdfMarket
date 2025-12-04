using PdfMarket.Contracts.Pdfs;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Abstractions.Repositories;

public interface IPdfRepository
{
    Task<PdfDocument?> GetByIdAsync(string id);
    Task<IReadOnlyCollection<PdfDocument>> BrowseAsync(PdfFilterRequest filter);
    Task AddAsync(PdfDocument pdf);
    Task UpdateAsync(PdfDocument pdf);
}
