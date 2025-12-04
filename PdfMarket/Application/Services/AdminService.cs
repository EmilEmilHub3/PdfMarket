using PdfMarket.Contracts.Admin;
using PdfMarket.Application.Abstractions.Repositories;

namespace PdfMarket.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository userRepository;
    private readonly IPdfRepository pdfRepository;
    private readonly IPurchaseRepository purchaseRepository;

    public AdminService(
        IUserRepository userRepository,
        IPdfRepository pdfRepository,
        IPurchaseRepository purchaseRepository)
    {
        this.userRepository = userRepository;
        this.pdfRepository = pdfRepository;
        this.purchaseRepository = purchaseRepository;
    }

    public async Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync()
    {
        var users = await userRepository.GetAllAsync();
        var purchases = await purchaseRepository.GetAllAsync();
        var allPdfs = await pdfRepository.BrowseAsync(
            new PdfMarket.Contracts.Pdfs.PdfFilterRequest(null, null, null, null));

        var summaries = users.Select(u =>
        {
            var uploads = allPdfs.Count(p => p.UploaderUserId == u.Id);
            var buys = purchases.Count(p => p.BuyerUserId == u.Id);

            return new UserSummaryDto(
                u.Id,
                u.UserName,
                u.Email,
                IsBlocked: false,    // no block feature yet
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
        var pdfs = await pdfRepository.BrowseAsync(
            new PdfMarket.Contracts.Pdfs.PdfFilterRequest(null, null, null, null));
        var purchases = await purchaseRepository.GetAllAsync();

        var totalPoints = users.Sum(u => u.PointsBalance);

        return new PlatformStatsDto(
            TotalUsers: users.Count,
            TotalPdfs: pdfs.Count,
            TotalPurchases: purchases.Count,
            TotalPointsInSystem: totalPoints
        );
    }
}
