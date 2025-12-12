using PdfMarket.Domain.Entities;
using PdfMarket.Contracts.Pdfs;

namespace PdfMarket.Application.Abstractions.Repositories;

/// <summary>
/// Repository abstraction for PDF document persistence.
/// </summary>
public interface IPdfRepository
{
    /// <summary>
    /// Returns a PDF by its identifier.
    /// </summary>
    Task<PdfDocument?> GetByIdAsync(string id);

    /// <summary>
    /// Returns publicly visible PDFs matching the provided filter.
    /// Only active PDFs are included.
    /// </summary>
    Task<IReadOnlyCollection<PdfDocument>> BrowseAsync(PdfFilterRequest filter);

    /// <summary>
    /// Returns active PDFs uploaded by a specific user.
    /// </summary>
    Task<IReadOnlyCollection<PdfDocument>> GetByUploaderAsync(string uploaderUserId);

    /// <summary>
    /// Returns all PDFs uploaded by a specific user, including inactive ones.
    /// Used for "My uploads".
    /// </summary>
    Task<IReadOnlyCollection<PdfDocument>> GetAllByUploaderAsync(string uploaderUserId);

    /// <summary>
    /// Returns all PDFs in the system (active and inactive).
    /// Used by admin functionality.
    /// </summary>
    Task<IReadOnlyCollection<PdfDocument>> GetAllAsync();

    /// <summary>
    /// Inserts a new PDF document.
    /// </summary>
    Task AddAsync(PdfDocument pdf);

    /// <summary>
    /// Updates an existing PDF document.
    /// </summary>
    Task UpdateAsync(PdfDocument pdf);

    /// <summary>
    /// Deletes a PDF document by identifier.
    /// </summary>
    Task DeleteAsync(string id);
}
