using System.Text;
using Moq;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Application.Abstractions.Storage;
using PdfMarket.Application.Services;
using PdfMarket.Domain.Entities;
using Xunit;

namespace PdfMarket.Tests;

public class PdfServiceDownloadTests
{
    [Fact]
    public async Task GetFileForDownloadAsync_ReturnsNull_WhenPdfNotFound()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var storage = new Mock<IFileStorage>();
        var purchaseService = new Mock<IPurchaseService>();

        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync((PdfDocument?)null);

        var sut = new PdfService(pdfRepo.Object, userRepo.Object, storage.Object, purchaseService.Object);

        var result = await sut.GetFileForDownloadAsync("u1", "pdf1");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetFileForDownloadAsync_AllowsUploader_WithoutPurchaseCheck()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var storage = new Mock<IFileStorage>();
        var purchaseService = new Mock<IPurchaseService>();

        var pdf = new PdfDocument
        {
            Id = "pdf1",
            UploaderUserId = "u1",
            IsActive = true,
            FileStorageId = "fs1",
            Title = "MyPdf",
            Description = "d"
        };

        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);

        storage.Setup(s => s.DownloadAsync("fs1", It.IsAny<Stream>()))
            .Callback<string, Stream>((_, target) =>
            {
                var bytes = Encoding.UTF8.GetBytes("fake-pdf-bytes");
                target.Write(bytes, 0, bytes.Length);
            })
            .Returns(Task.CompletedTask);

        var sut = new PdfService(pdfRepo.Object, userRepo.Object, storage.Object, purchaseService.Object);

        var result = await sut.GetFileForDownloadAsync("u1", "pdf1");

        Assert.NotNull(result);
        Assert.Equal("MyPdf.pdf", result!.FileName);

        purchaseService.Verify(p => p.HasUserPurchasedPdfAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        storage.Verify(s => s.DownloadAsync("fs1", It.IsAny<Stream>()), Times.Once);
    }

    [Fact]
    public async Task GetFileForDownloadAsync_ReturnsNull_WhenNotUploader_AndNotPurchased()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var storage = new Mock<IFileStorage>();
        var purchaseService = new Mock<IPurchaseService>();

        var pdf = new PdfDocument
        {
            Id = "pdf1",
            UploaderUserId = "seller",
            IsActive = true,
            FileStorageId = "fs1",
            Title = "MyPdf",
            Description = "d"
        };

        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);
        purchaseService.Setup(p => p.HasUserPurchasedPdfAsync("buyer", "pdf1")).ReturnsAsync(false);

        var sut = new PdfService(pdfRepo.Object, userRepo.Object, storage.Object, purchaseService.Object);

        var result = await sut.GetFileForDownloadAsync("buyer", "pdf1");

        Assert.Null(result);

        storage.Verify(s => s.DownloadAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Never);
    }

    [Fact]
    public async Task GetFileForDownloadAsync_AllowsDownload_WhenPurchased()
    {
        var pdfRepo = new Mock<IPdfRepository>();
        var userRepo = new Mock<IUserRepository>();
        var storage = new Mock<IFileStorage>();
        var purchaseService = new Mock<IPurchaseService>();

        var pdf = new PdfDocument
        {
            Id = "pdf1",
            UploaderUserId = "seller",
            IsActive = true,
            FileStorageId = "fs1",
            Title = "MyPdf",
            Description = "d"
        };

        pdfRepo.Setup(r => r.GetByIdAsync("pdf1")).ReturnsAsync(pdf);
        purchaseService.Setup(p => p.HasUserPurchasedPdfAsync("buyer", "pdf1")).ReturnsAsync(true);

        storage.Setup(s => s.DownloadAsync("fs1", It.IsAny<Stream>()))
            .Callback<string, Stream>((_, target) =>
            {
                var bytes = Encoding.UTF8.GetBytes("fake-pdf-bytes");
                target.Write(bytes, 0, bytes.Length);
            })
            .Returns(Task.CompletedTask);

        var sut = new PdfService(pdfRepo.Object, userRepo.Object, storage.Object, purchaseService.Object);

        var result = await sut.GetFileForDownloadAsync("buyer", "pdf1");

        Assert.NotNull(result);
        storage.Verify(s => s.DownloadAsync("fs1", It.IsAny<Stream>()), Times.Once);
    }
}
