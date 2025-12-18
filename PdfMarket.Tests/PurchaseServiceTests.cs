using Moq;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Application.Services;
using PdfMarket.Contracts.Purchases;
using PdfMarket.Domain.Entities;
using Xunit;

namespace PdfMarket.Tests;

/// <summary>
/// Unit tests for PurchaseService.PurchaseAsync.
/// The tests cover the main control-flow branches identified
/// in the cyclomatic complexity analysis.
/// </summary>
public class PurchaseServiceTests
{
    private PurchaseService CreateSut(
        Mock<IPdfRepository> pdfRepo,
        Mock<IUserRepository> userRepo,
        Mock<IPurchaseRepository> purchaseRepo)
    {
        return new PurchaseService(
            pdfRepo.Object,
            userRepo.Object,
            purchaseRepo.Object
        );
    }

    /// <summary>
    /// Buyer does not exist → early return.
    /// </summary>
    [Fact]
    public async Task PurchaseAsync_ReturnsNull_WhenBuyerNotFound()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        userRepo.Setup(r => r.GetByIdAsync("buyer"))
            .ReturnsAsync((User?)null);

        var sut = CreateSut(pdfRepo, userRepo, purchaseRepo);

        var result = await sut.PurchaseAsync("buyer", new PurchaseRequest("pdf1"));

        Assert.Null(result);
    }

    /// <summary>
    /// PDF does not exist → early return.
    /// </summary>
    [Fact]
    public async Task PurchaseAsync_ReturnsNull_WhenPdfNotFound()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        var buyer = new User
        {
            Id = "buyer",
            UserName = "b",
            Email = "b@x",
            PasswordHash = "pw",
            PointsBalance = 50
        };

        userRepo.Setup(r => r.GetByIdAsync("buyer")).ReturnsAsync(buyer);
        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync((PdfDocument?)null);

        var sut = CreateSut(pdfRepo, userRepo, purchaseRepo);

        var result = await sut.PurchaseAsync("buyer", new PurchaseRequest("pdf1"));

        Assert.Null(result);
    }

    /// <summary>
    /// PDF exists but is inactive → early return.
    /// </summary>
    [Fact]
    public async Task PurchaseAsync_ReturnsNull_WhenPdfIsInactive()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        var buyer = new User
        {
            Id = "buyer",
            UserName = "b",
            Email = "b@x",
            PasswordHash = "pw",
            PointsBalance = 50
        };

        var pdf = new PdfDocument
        {
            Id = "pdf1",
            UploaderUserId = "seller",
            PriceInPoints = 10,
            IsActive = false,
            Title = "t",
            Description = "d"
        };

        userRepo.Setup(r => r.GetByIdAsync("buyer")).ReturnsAsync(buyer);
        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);

        var sut = CreateSut(pdfRepo, userRepo, purchaseRepo);

        var result = await sut.PurchaseAsync("buyer", new PurchaseRequest("pdf1"));

        Assert.Null(result);
    }

    /// <summary>
    /// Buyer does not have sufficient points → exception.
    /// </summary>
    [Fact]
    public async Task PurchaseAsync_Throws_WhenBuyerHasNotEnoughPoints()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        var buyer = new User
        {
            Id = "buyer",
            UserName = "b",
            Email = "b@x",
            PasswordHash = "pw",
            PointsBalance = 0
        };

        var seller = new User
        {
            Id = "seller",
            UserName = "s",
            Email = "s@x",
            PasswordHash = "pw",
            PointsBalance = 0
        };

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
        userRepo.Setup(r => r.GetByIdAsync("seller")).ReturnsAsync(seller);
        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);

        var sut = CreateSut(pdfRepo, userRepo, purchaseRepo);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.PurchaseAsync("buyer", new PurchaseRequest("pdf1")));
    }

    /// <summary>
    /// Seller cannot be resolved → exception.
    /// </summary>
    [Fact]
    public async Task PurchaseAsync_Throws_WhenSellerNotFound()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        var buyer = new User
        {
            Id = "buyer",
            UserName = "b",
            Email = "b@x",
            PasswordHash = "pw",
            PointsBalance = 50
        };

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
        userRepo.Setup(r => r.GetByIdAsync("seller")).ReturnsAsync((User?)null);
        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);

        var sut = CreateSut(pdfRepo, userRepo, purchaseRepo);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.PurchaseAsync("buyer", new PurchaseRequest("pdf1")));
    }

    /// <summary>
    /// Valid purchase → points transferred and purchase created.
    /// </summary>
    [Fact]
    public async Task PurchaseAsync_CompletesPurchase_WhenValid()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        var buyer = new User
        {
            Id = "buyer",
            UserName = "b",
            Email = "b@x",
            PasswordHash = "pw",
            PointsBalance = 50
        };

        var seller = new User
        {
            Id = "seller",
            UserName = "s",
            Email = "s@x",
            PasswordHash = "pw",
            PointsBalance = 5
        };

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
        userRepo.Setup(r => r.GetByIdAsync("seller")).ReturnsAsync(seller);
        userRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);
        purchaseRepo.Setup(r => r.AddAsync(It.IsAny<Purchase>())).Returns(Task.CompletedTask);

        var sut = CreateSut(pdfRepo, userRepo, purchaseRepo);

        var result = await sut.PurchaseAsync("buyer", new PurchaseRequest("pdf1"));

        Assert.NotNull(result);
        Assert.Equal(40, buyer.PointsBalance);
        Assert.Equal(15, seller.PointsBalance);
        Assert.Contains("pdf1", buyer.OwnedPdfIds);
    }

    /// <summary>
    /// Self-purchase → buyer debited, seller not credited.
    /// </summary>
    [Fact]
    public async Task PurchaseAsync_DoesNotCreditSeller_WhenSelfPurchase()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var purchaseRepo = new Mock<IPurchaseRepository>();

        var user = new User
        {
            Id = "u1",
            UserName = "u",
            Email = "u@x",
            PasswordHash = "pw",
            PointsBalance = 20
        };

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
        userRepo.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);
        purchaseRepo.Setup(r => r.AddAsync(It.IsAny<Purchase>())).Returns(Task.CompletedTask);

        var sut = CreateSut(pdfRepo, userRepo, purchaseRepo);

        var result = await sut.PurchaseAsync("u1", new PurchaseRequest("pdf1"));

        Assert.NotNull(result);
        Assert.Equal(10, user.PointsBalance);
    }
}
