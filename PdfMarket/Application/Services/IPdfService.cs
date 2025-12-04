using PdfMarket.Contracts.Pdfs;

namespace PdfMarket.Application.Services;

public interface IPdfService
{
    Task<IReadOnlyCollection<PdfSummaryDto>> BrowseAsync(PdfFilterRequest filter);
    Task<PdfDetailsDto?> GetDetailsAsync(string id);
    Task<PdfDetailsDto> UploadAsync(string userId, UploadPdfRequest request, Stream pdfStream, string fileName);
    Task<PdfDetailsDto?> UpdateAsync(string userId, string pdfId, UpdatePdfRequest request);
    Task<bool> DeactivateAsync(string userId, string pdfId);
}
