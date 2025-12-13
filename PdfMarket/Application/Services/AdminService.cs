using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Application.Abstractions.Storage;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.Application.Services;

/// <summary>
/// Implements admin-only operations like user overview, moderation, and statistics.
/// </summary>
public class AdminService : IAdminService
{
    private readonly IUserRepository userRepository;
    private readonly IPdfRepository pdfRepository;
    private readonly IPurchaseRepository purchaseRepository;
    private readonly IFileStorage fileStorage;

    public AdminService(
        IUserRepository userRepository,
        IPdfRepository pdfRepository,
        IPurchaseRepository purchaseRepository,
        IFileStorage fileStorage)
    {
        this.userRepository = userRepository;
        this.pdfRepository = pdfRepository;
        this.purchaseRepository = purchaseRepository;
        this.fileStorage = fileStorage;
    }

    public async Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync()
    {
        // Load data sets used for computed columns (uploads/purchases).
        var users = await userRepository.GetAllAsync();
        var purchases = await purchaseRepository.GetAllAsync();
        var allPdfs = await pdfRepository.GetAllAsync(); // includes active + inactive

        return users.Select(u =>
        {
            // Compute simple admin stats without extra DB roundtrips.
            var uploads = allPdfs.Count(p => p.UploaderUserId == u.Id);
            var buys = purchases.Count(p => p.BuyerUserId == u.Id);

            return new UserSummaryDto(
                u.Id,
                u.UserName,
                u.Email,
                IsBlocked: false, // Project scope: blocking not implemented
                u.PointsBalance,
                uploads,
                buys
            );
        }).ToList();
    }

    public async Task<PlatformStatsDto> GetStatsAsync()
    {
        var users = await userRepository.GetAllAsync();
        var pdfs = await pdfRepository.GetAllAsync(); // includes active + inactive
        var purchases = await purchaseRepository.GetAllAsync();

        // Useful “health metric” for the points economy.
        var totalPoints = users.Sum(u => u.PointsBalance);

        return new PlatformStatsDto(
            TotalUsers: users.Count,
            TotalPdfs: pdfs.Count,
            TotalPurchases: purchases.Count,
            TotalPointsInSystem: totalPoints
        );
    }

    public async Task<IReadOnlyCollection<AdminPdfListItemDto>> GetPdfsAsync()
    {
        // Admin needs visibility across all uploads (active + inactive).
        var pdfs = await pdfRepository.GetAllAsync();
        var users = await userRepository.GetAllAsync();

        return pdfs.Select(p =>
        {
            var uploader = users.FirstOrDefault(u => u.Id == p.UploaderUserId);

            return new AdminPdfListItemDto(
                p.Id,
                p.Title,
                p.UploaderUserId,
                uploader?.UserName ?? "Unknown",
                p.PriceInPoints,
                p.Tags,
                p.CreatedAt,
                p.IsActive
            );
        }).ToList();
    }

    public async Task<bool> DeletePdfAsync(string pdfId)
    {
        var pdf = await pdfRepository.GetByIdAsync(pdfId);
        if (pdf is null)
            return false;

        // Delete the stored file first (best-effort), then delete metadata.
        if (!string.IsNullOrWhiteSpace(pdf.FileStorageId))
        {
            await fileStorage.DeleteAsync(pdf.FileStorageId);
        }

        await pdfRepository.DeleteAsync(pdfId);
        return true;
    }

    public async Task<bool> ResetUserPasswordAsync(string userId, string newPassword)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        if (string.IsNullOrWhiteSpace(newPassword))
            return false;

        // Project scope: passwords are stored/compared as-is (replace with hashing in production).
        user.PasswordHash = newPassword;

        await userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null) return false;

        // Kun opdater det der er sendt med
        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email;

        if (request.PointsBalance.HasValue)
            user.PointsBalance = request.PointsBalance.Value;

        await userRepository.UpdateAsync(user);
        return true;
    }


}
