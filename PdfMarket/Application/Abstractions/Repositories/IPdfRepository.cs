using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Abstractions.Repositories;

public interface IPdfRepository
{
    Task<PdfDocument?> GetByIdAsync(string id);
    Task<IReadOnlyCollection<PdfDocument>> BrowseAsync(PdfMarket.Contracts.Pdfs.PdfFilterRequest filter);

    // ADMIN
    Task<IReadOnlyCollection<PdfDocument>> GetAllAsync();
    Task DeleteAsync(string id);

    Task AddAsync(PdfDocument pdf);
    Task UpdateAsync(PdfDocument pdf);
}
