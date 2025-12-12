using System.IO;
using PdfMarket.Contracts.Pdfs;

namespace PdfMarket.Application.Services;

/// <summary>
/// Application service for PDF operations (browse, details, upload, manage uploads, and downloads).
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Returns a list of publicly visible PDFs matching the given filter.
    /// </summary>
    Task<IReadOnlyCollection<PdfSummaryDto>> BrowseAsync(PdfFilterRequest filter);

    /// <summary>
    /// Returns detailed information about a specific PDF, or null if not found.
    /// </summary>
    Task<PdfDetailsDto?> GetDetailsAsync(string id);

    /// <summary>
    /// Uploads a new PDF and returns upload result including updated uploader points balance.
    /// </summary>
    Task<UploadPdfResponse> UploadAsync(
        string userId,
        UploadPdfRequest request,
        Stream pdfStream,
        string fileName);

    /// <summary>
    /// Updates an existing PDF (metadata + visibility). Only the uploader may update.
    /// Returns updated details, or null if not found/unauthorized.
    /// </summary>
    Task<PdfDetailsDto?> UpdateAsync(string userId, string pdfId, UpdatePdfRequest request);

    /// <summary>
    /// Deactivates a PDF (makes it invisible in public browsing). Only the uploader may deactivate.
    /// </summary>
    Task<bool> DeactivateAsync(string userId, string pdfId);

    /// <summary>
    /// Returns all PDFs uploaded by the user, including inactive ones (so they can be re-enabled).
    /// </summary>
    Task<IReadOnlyCollection<PdfSummaryDto>> GetMyUploadsAsync(string userId);

    /// <summary>
    /// Returns the PDF file for download, or null if it cannot be downloaded.
    /// Typical reasons: not found, inactive, missing file, or user is not allowed.
    /// </summary>
    Task<PdfFileResult?> GetFileForDownloadAsync(string userId, string pdfId);
}

/// <summary>
/// Result object for a downloadable PDF file.
/// </summary>
public record PdfFileResult(Stream Stream, string FileName, string ContentType);
