namespace PdfMarket.Contracts.Pdfs;

/// <summary>
/// Request payload used when uploading a PDF.
/// </summary>
public class UploadPdfRequest
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int PriceInPoints { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Request payload for updating PDF metadata.
/// </summary>
public record UpdatePdfRequest(
    string Title,
    string Description,
    int PriceInPoints,
    string[] Tags,
    bool IsActive
);

/// <summary>
/// Filter used for browsing/searching PDFs.
/// </summary>
public record PdfFilterRequest(
    string? Query,
    string? Tag,
    int? MinPriceInPoints,
    int? MaxPriceInPoints
);

/// <summary>
/// Lightweight DTO used for listing PDFs.
/// </summary>
public record PdfSummaryDto(
    string Id,
    string Title,
    string UploaderUserName,
    int PriceInPoints,
    string[] Tags
);

/// <summary>
/// Detailed DTO used for PDF detail pages.
/// </summary>
public record PdfDetailsDto(
    string Id,
    string Title,
    string Description,
    string UploaderUserName,
    int PriceInPoints,
    string[] Tags,
    int PointsReward,
    DateTime CreatedAt,
    bool IsActive
);

/// <summary>
/// Response returned after uploading a PDF, including updated uploader points.
/// </summary>
public record UploadPdfResponse(
    PdfDetailsDto Pdf,
    int UploaderPointsBalance
);
