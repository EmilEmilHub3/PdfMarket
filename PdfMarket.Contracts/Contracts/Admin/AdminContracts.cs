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

public record AdminPdfListItemDto(
    string Id,
    string Title,
    string UploaderUserId,
    string UploaderUserName,
    int PriceInPoints,
    string[] Tags,
    DateTime CreatedAt,
    bool IsActive
);


public record UpdateUserRequest(
    string? Email,
    int? PointsBalance
);