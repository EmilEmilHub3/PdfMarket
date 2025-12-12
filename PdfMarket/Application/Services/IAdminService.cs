using PdfMarket.Contracts.Admin;

namespace PdfMarket.Application.Services;

public interface IAdminService
{
    Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync();
    Task<PlatformStatsDto> GetStatsAsync();

    Task<IReadOnlyCollection<AdminPdfListItemDto>> GetPdfsAsync();
    Task<bool> DeletePdfAsync(string pdfId);
}
