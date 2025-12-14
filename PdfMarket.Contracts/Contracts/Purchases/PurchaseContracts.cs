namespace PdfMarket.Contracts.Purchases;

/// <summary>
/// Request payload for purchasing a PDF.
/// </summary>
public record PurchaseRequest(string PdfId);

/// <summary>
/// Response returned after a successful purchase.
/// </summary>
public record PurchaseResponse(
    string PurchaseId,
    string PdfId,
    string BuyerUserId,
    DateTime PurchasedAt,
    int PriceInPoints,
    int BuyerPointsBalance
);

/// <summary>
/// DTO used for listing a user's purchases with enriched PDF info.
/// </summary>
public record PurchasedPdfDto(
    string PdfId,
    string Title,
    int PriceInPoints,
    DateTime PurchasedAt
);
