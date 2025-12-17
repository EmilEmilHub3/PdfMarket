using Moq;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Application.Services;
using PdfMarket.Contracts.Purchases;
using PdfMarket.Domain.Entities;
using Xunit;

namespace PdfMarket.Tests;

public class PurchaseServiceTests
{
    [Fact]
    public async Task PurchaseAsync_ReturnsNull_WhenBuyerNotFound()
    {
        var pdfRepo = new Mock<IPdfRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var purchaseRepo = new Mock<IPurchaseRepository>(MockBehavior.Strict);

        userRepo.Setup(r => r.GetByIdAsync("buyer")).ReturnsAsync((User?)null);

        var sut = new PurchaseService(pdfRepo.Object, userRepo.Object, purchaseRepo.Object);

        var result = await sut.PurchaseAsync("buyer", new PurchaseRequest("pdf1"));

        Assert.Null(result);
    }

    [Fact]
    public async Task PurchaseAsync_Throws_WhenBuyerHasNotEnoughPoints()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        var buyer = new User { Id = "buyer", UserName = "b", Email = "b@x", PasswordHash = "pw", PointsBalance = 0 };
        var pdf = new PdfDocument { Id = "pdf1", UploaderUserId = "seller", PriceInPoints = 10, IsActive = true, Title = "t", Description = "d" };
        var seller = new User { Id = "seller", UserName = "s", Email = "s@x", PasswordHash = "pw", PointsBalance = 0 };

        userRepo.Setup(r => r.GetByIdAsync("buyer")).ReturnsAsync(buyer);
        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);
        userRepo.Setup(r => r.GetByIdAsync("seller")).ReturnsAsync(seller);

        var sut = new PurchaseService(pdfRepo.Object, userRepo.Object, purchaseRepo.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.PurchaseAsync("buyer", new PurchaseRequest("pdf1")));
    }

    [Fact]
    public async Task PurchaseAsync_TransfersPoints_AndCreatesPurchase_WhenValid()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        var buyer = new User { Id = "buyer", UserName = "b", Email = "b@x", PasswordHash = "pw", PointsBalance = 50 };
        var seller = new User { Id = "seller", UserName = "s", Email = "s@x", PasswordHash = "pw", PointsBalance = 5 };
        var pdf = new PdfDocument
        {
            Id = "pdf1",
            UploaderUserId = "seller",
            PriceInPoints = 10,
            IsActive = true,
            Title = "t",
            Description = "d"
        };

        userRepo.Setup(r => r.GetByIdAsync("buyer")).ReturnsAsync(buyer);
        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);
        userRepo.Setup(r => r.GetByIdAsync("seller")).ReturnsAsync(seller);

        userRepo.Setup(r => r.UpdateAsync(buyer)).Returns(Task.CompletedTask);
        userRepo.Setup(r => r.UpdateAsync(seller)).Returns(Task.CompletedTask);

        Purchase? savedPurchase = null;
        purchaseRepo.Setup(r => r.AddAsync(It.IsAny<Purchase>()))
            .Callback<Purchase>(p => savedPurchase = p)
            .Returns(Task.CompletedTask);

        var sut = new PurchaseService(pdfRepo.Object, userRepo.Object, purchaseRepo.Object);

        var result = await sut.PurchaseAsync("buyer", new PurchaseRequest("pdf1"));

        Assert.NotNull(result);
        Assert.Equal(40, buyer.PointsBalance);              // 50 - 10
        Assert.Equal(15, seller.PointsBalance);             // 5 + 10
        Assert.Contains("pdf1", buyer.OwnedPdfIds);

        Assert.NotNull(savedPurchase);
        Assert.Equal("buyer", savedPurchase!.BuyerUserId);
        Assert.Equal("pdf1", savedPurchase.PdfId);
        Assert.Equal(10, savedPurchase.PriceInPoints);
    }

    [Fact]
    public async Task PurchaseAsync_DoesNotCreditSeller_WhenBuyerBuysOwnPdf()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        var user = new User { Id = "u1", UserName = "u", Email = "u@x", PasswordHash = "pw", PointsBalance = 20 };
        var pdf = new PdfDocument
        {
            Id = "pdf1",
            UploaderUserId = "u1",
            PriceInPoints = 10,
            IsActive = true,
            Title = "t",
            Description = "d"
        };

        userRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);

        // Seller lookup returns same user (uploader == buyer)
        userRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);

        userRepo.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
        purchaseRepo.Setup(r => r.AddAsync(It.IsAny<Purchase>())).Returns(Task.CompletedTask);

        var sut = new PurchaseService(pdfRepo.Object, userRepo.Object, purchaseRepo.Object);

        var result = await sut.PurchaseAsync("u1", new PurchaseRequest("pdf1"));

        Assert.NotNull(result);
        Assert.Equal(10, user.PointsBalance); // debit still happens
        // No double-credit possible because same user
    }
}
