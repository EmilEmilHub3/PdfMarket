namespace PdfMarket.Contracts.Admin;

public record UserSummaryDto(
    string Id,
    string UserName,
    string Email,
    bool IsBlocked,       // Not implemented yet
    int PointsBalance,
    int TotalUploads,
    int TotalPurchases
);

public record PlatformStatsDto(
    int TotalUsers,
    int TotalPdfs,
    int TotalPurchases,
    int TotalPointsInSystem
);
