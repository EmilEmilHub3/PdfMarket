namespace PdfMarket.Contracts.Purchases;

public record PurchaseRequest(
    string PdfId
);

public record PurchaseResponse(
    string PurchaseId,
    string PdfId,
    string BuyerUserId,
    DateTime PurchasedAt,
    int PriceInPoints
);
