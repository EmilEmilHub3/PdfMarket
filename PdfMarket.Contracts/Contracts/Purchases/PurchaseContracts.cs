namespace PdfMarket.Contracts.Purchases;

public record PurchaseRequest(string PdfId);

public record PurchaseResponse(
    string PurchaseId,
    string PdfId,
    string BuyerUserId,
    DateTime PurchasedAt,
    int PriceInPoints,
    int BuyerPointsBalance
);

public record PurchasedPdfDto(
    string PdfId,
    string Title,
    int PriceInPoints,
    DateTime PurchasedAt
);
