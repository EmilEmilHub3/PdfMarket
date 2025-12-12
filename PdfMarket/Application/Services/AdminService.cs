using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Application.Abstractions.Storage;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.Application.Services;

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
        var users = await userRepository.GetAllAsync();
        var purchases = await purchaseRepository.GetAllAsync();

        // Admin: count ALL PDFs (active + inactive)
        var allPdfs = await pdfRepository.GetAllAsync();

        var summaries = users.Select(u =>
        {
            var uploads = allPdfs.Count(p => p.UploaderUserId == u.Id);
            var buys = purchases.Count(p => p.BuyerUserId == u.Id);

            return new UserSummaryDto(
                u.Id,
                u.UserName,
                u.Email,
                IsBlocked: false, // no block feature yet
                u.PointsBalance,
                uploads,
                buys
            );
        }).ToList();

        return summaries;
    }

    public async Task<PlatformStatsDto> GetStatsAsync()
    {
        var users = await userRepository.GetAllAsync();

        // Admin: total PDFs should be ALL (active + inactive)
        var pdfs = await pdfRepository.GetAllAsync();

        var purchases = await purchaseRepository.GetAllAsync();

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

        if (!string.IsNullOrWhiteSpace(pdf.FileStorageId))
        {
            await fileStorage.DeleteAsync(pdf.FileStorageId);
        }

        await pdfRepository.DeleteAsync(pdfId);
        return true;
    }

    // NEW: reset password (matches your current AuthService behavior)
    public async Task<bool> ResetUserPasswordAsync(string userId, string newPassword)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        if (string.IsNullOrWhiteSpace(newPassword))
            return false;

        // Your AuthService currently compares PasswordHash to plaintext password.
        // So keep this consistent for now.
        user.PasswordHash = newPassword;

        await userRepository.UpdateAsync(user);
        return true;
    }
}
