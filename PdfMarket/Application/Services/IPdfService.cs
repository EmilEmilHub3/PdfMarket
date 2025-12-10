using System.IO;
using PdfMarket.Contracts.Pdfs;

namespace PdfMarket.Application.Services;

public interface IPdfService
{
    Task<IReadOnlyCollection<PdfSummaryDto>> BrowseAsync(PdfFilterRequest filter);
    Task<PdfDetailsDto?> GetDetailsAsync(string id);
    Task<UploadPdfResponse> UploadAsync(string userId, UploadPdfRequest request, Stream pdfStream, string fileName);

    Task<PdfDetailsDto?> UpdateAsync(string userId, string pdfId, UpdatePdfRequest request);
    Task<bool> DeactivateAsync(string userId, string pdfId);

    /// <summary>
    /// Returns the PDF file for download, or null if it cannot be downloaded.
    /// </summary>
    Task<PdfFileResult?> GetFileForDownloadAsync(string userId, string pdfId);
}

/// <summary>
/// Result object for a downloadable PDF file.
/// </summary>
public record PdfFileResult(Stream Stream, string FileName, string ContentType);
