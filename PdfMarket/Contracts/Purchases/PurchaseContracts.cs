// src/Contracts/Purchases/PurchaseContracts.cs
namespace PdfMarket.Contracts.Purchases;

public record PurchaseRequest(
    string PdfId
);

public record PurchaseResponse(
    string PurchaseId,
    string PdfId,
    string BuyerUserId,
    DateTime PurchasedAt,
    int PriceInPoints,
    int BuyerPointsBalance
);

// NEW: used for "My purchases" / downloads list
public record PurchasedPdfDto(
    string PdfId,
    string Title,
    int PriceInPoints,
    DateTime PurchasedAt
);
