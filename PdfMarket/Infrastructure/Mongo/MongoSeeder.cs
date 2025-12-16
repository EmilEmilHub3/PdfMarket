using MongoDB.Driver;
using PdfMarket.Application.Abstractions.Storage;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.Mongo;

public class MongoSeeder
{
    private readonly IMongoDatabase db;
    private readonly IFileStorage fileStorage;

    public MongoSeeder(IMongoDatabase db, IFileStorage fileStorage)
    {
        this.db = db;
        this.fileStorage = fileStorage;
    }

    public async Task SeedAsync()
    {
        var users = db.GetCollection<User>("users");
        var pdfs = db.GetCollection<PdfDocument>("pdfs");
        var purchases = db.GetCollection<Purchase>("purchases");

        // Seed kun hvis DB er "tom" (du kan vælge kun at tjekke users, men her tjekker vi alle tre)
        var hasUsers = await users.Find(_ => true).AnyAsync();
        var hasPdfs = await pdfs.Find(_ => true).AnyAsync();
        var hasPurchases = await purchases.Find(_ => true).AnyAsync();

        if (hasUsers || hasPdfs || hasPurchases)
            return;

        // 1) Users (3 stk, 1 admin)
        var admin = new User
        {
            UserName = "admin",
            Email = "admin@pdfmarket.local",
            PasswordHash = "Admin123!", // scope: plain text i dit projekt (men hash i prod)
            Role = "Admin",
            PointsBalance = 9999
        };

        var user1 = new User
        {
            UserName = "alice",
            Email = "alice@pdfmarket.local",
            PasswordHash = "Alice123!",
            Role = "User",
            PointsBalance = 300
        };

        var user2 = new User
        {
            UserName = "bob",
            Email = "bob@pdfmarket.local",
            PasswordHash = "Bob123!",
            Role = "User",
            PointsBalance = 300
        };

        await users.InsertManyAsync([admin, user1, user2]);

        // 2) PDFs (3 stk) + GridFS filer
        // Minimal "PDF" bytes (nok til at være en fil; ikke vigtig for testen)
        static byte[] FakePdfBytes(string title)
        {
            var text = $"%PDF-1.4\n% Fake PDF for seed: {title}\n1 0 obj\n<<>>\nendobj\ntrailer\n<<>>\n%%EOF\n";
            return System.Text.Encoding.UTF8.GetBytes(text);
        }

        async Task<string> UploadSeedPdfAsync(string title)
        {
            var bytes = FakePdfBytes(title);
            using var ms = new MemoryStream(bytes);
            return await fileStorage.UploadAsync(ms, $"{title}.pdf", "application/pdf");
        }

        var pdf1StorageId = await UploadSeedPdfAsync("Clean Architecture Notes");
        var pdf2StorageId = await UploadSeedPdfAsync("MongoDB Cheat Sheet");
        var pdf3StorageId = await UploadSeedPdfAsync("JWT Quick Guide");

        var pdf1 = new PdfDocument
        {
            Title = "Clean Architecture Notes",
            Description = "Seeded PDF: short notes about layers and boundaries.",
            UploaderUserId = admin.Id,
            PriceInPoints = 50,
            Tags = ["architecture", "clean", "notes"],
            IsActive = true,
            FileStorageId = pdf1StorageId
        };

        var pdf2 = new PdfDocument
        {
            Title = "MongoDB Cheat Sheet",
            Description = "Seeded PDF: common queries and patterns.",
            UploaderUserId = user1.Id,
            PriceInPoints = 75,
            Tags = ["mongodb", "database"],
            IsActive = true,
            FileStorageId = pdf2StorageId
        };

        var pdf3 = new PdfDocument
        {
            Title = "JWT Quick Guide",
            Description = "Seeded PDF: JWT basics + roles.",
            UploaderUserId = user2.Id,
            PriceInPoints = 60,
            Tags = ["jwt", "security", "auth"],
            IsActive = true,
            FileStorageId = pdf3StorageId
        };

        await pdfs.InsertManyAsync([pdf1, pdf2, pdf3]);

        // 3) Purchases (3 stk)
        // Lav nogle køb (kryds-køb) + opdater OwnedPdfIds + points
        var purchase1 = new Purchase
        {
            PdfId = pdf1.Id,
            BuyerUserId = user1.Id,
            PriceInPoints = pdf1.PriceInPoints,
            PurchasedAt = DateTime.UtcNow.AddMinutes(-30)
        };

        var purchase2 = new Purchase
        {
            PdfId = pdf2.Id,
            BuyerUserId = user2.Id,
            PriceInPoints = pdf2.PriceInPoints,
            PurchasedAt = DateTime.UtcNow.AddMinutes(-20)
        };

        var purchase3 = new Purchase
        {
            PdfId = pdf3.Id,
            BuyerUserId = admin.Id,
            PriceInPoints = pdf3.PriceInPoints,
            PurchasedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        await purchases.InsertManyAsync([purchase1, purchase2, purchase3]);

        // OwnedPdfIds + points-balance (simpelt seed-regnestykke)
        user1.OwnedPdfIds.Add(pdf1.Id);
        user2.OwnedPdfIds.Add(pdf2.Id);
        admin.OwnedPdfIds.Add(pdf3.Id);

        user1.PointsBalance -= pdf1.PriceInPoints;
        user2.PointsBalance -= pdf2.PriceInPoints;
        admin.PointsBalance -= pdf3.PriceInPoints;

        await users.ReplaceOneAsync(u => u.Id == user1.Id, user1);
        await users.ReplaceOneAsync(u => u.Id == user2.Id, user2);
        await users.ReplaceOneAsync(u => u.Id == admin.Id, admin);
    }
}
