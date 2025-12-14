namespace PdfMarket.Contracts.Admin;

/// <summary>
/// Summary information used by the admin client to display users.
/// </summary>
public record UserSummaryDto(
    string Id,
    string UserName,
    string Email,
    bool IsBlocked,
    int PointsBalance,
    int TotalUploads,
    int TotalPurchases
);

/// <summary>
/// Aggregated statistics for the platform.
/// </summary>
public record PlatformStatsDto(
    int TotalUsers,
    int TotalPdfs,
    int TotalPurchases,
    int TotalPointsInSystem
);

/// <summary>
/// Summary information used by the admin client to moderate PDFs.
/// </summary>
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

/// <summary>
/// Request payload for updating a user. Nullable fields mean "only update what is provided".
/// </summary>
public record UpdateUserRequest(
    string? Email,
    int? PointsBalance
);

/// <summary>
/// Request payload for resetting a user's password.
/// </summary>
public record ResetPasswordRequest(string NewPassword);
