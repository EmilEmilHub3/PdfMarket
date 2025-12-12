using PdfMarket.Contracts.Admin;

namespace PdfMarket.Application.Services;

/// <summary>
/// Application service for admin-only functionality (users, PDFs, and platform statistics).
/// </summary>
public interface IAdminService
{
    /// <summary>
    /// Returns an overview of all users including basic statistics (uploads, purchases, points).
    /// </summary>
    Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync();

    /// <summary>
    /// Returns platform-wide statistics (users, PDFs, purchases, total points).
    /// </summary>
    Task<PlatformStatsDto> GetStatsAsync();

    /// <summary>
    /// Returns a list of all PDFs (active and inactive) for moderation purposes.
    /// </summary>
    Task<IReadOnlyCollection<AdminPdfListItemDto>> GetPdfsAsync();

    /// <summary>
    /// Deletes a PDF and its stored file (if any). Returns false if not found.
    /// </summary>
    Task<bool> DeletePdfAsync(string pdfId);

    /// <summary>
    /// Resets a user's password. Returns false if user not found or password invalid.
    /// </summary>
    Task<bool> ResetUserPasswordAsync(string userId, string newPassword);
}
