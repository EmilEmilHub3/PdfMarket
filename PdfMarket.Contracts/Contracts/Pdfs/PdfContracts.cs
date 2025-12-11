namespace PdfMarket.Contracts.Pdfs
{
    public class UploadPdfRequest
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int PriceInPoints { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
    }

    public record UpdatePdfRequest(
        string Title,
        string Description,
        int PriceInPoints,
        string[] Tags,
        bool IsActive
    );

    public record PdfFilterRequest(
        string? Query,
        string? Tag,
        int? MinPriceInPoints,
        int? MaxPriceInPoints
    );

    public record PdfSummaryDto(
        string Id,
        string Title,
        string UploaderUserName,
        int PriceInPoints,
        string[] Tags
    );

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

    // 🔹 NYT: Response når der uploades en PDF
    public record UploadPdfResponse(
        PdfDetailsDto Pdf,
        int UploaderPointsBalance
    );
}
