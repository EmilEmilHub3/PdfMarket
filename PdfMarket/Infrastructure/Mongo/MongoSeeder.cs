using MongoDB.Driver;
using PdfMarket.Application.Abstractions.Storage;
using PdfMarket.Domain.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PdfMarket.Infrastructure.Mongo;

/// <summary>
/// Responsible for seeding initial demo data into MongoDB.
///
/// Purpose:
/// - Provides predictable test data for development and exam demonstration.
/// - Ensures the system starts with users, PDFs and purchases.
/// - Uploads PDF files via the abstracted file storage (GridFS).
///
/// This seeder is intentionally simple and only runs if the database is empty.
/// </summary>
public class MongoSeeder
{
    private readonly IMongoDatabase db;
    private readonly IFileStorage fileStorage;

    /// <summary>
    /// Creates a new MongoSeeder.
    /// </summary>
    /// <param name="db">MongoDB database instance.</param>
    /// <param name="fileStorage">
    /// File storage abstraction used to store PDF files (GridFS implementation).
    /// </param>
    public MongoSeeder(IMongoDatabase db, IFileStorage fileStorage)
    {
        this.db = db;
        this.fileStorage = fileStorage;
    }

    /// <summary>
    /// Seeds the database with initial data if it is empty.
    ///
    /// Seeded data includes:
    /// - 3 users (1 admin, 2 regular users)
    /// - 3 PDF documents with files stored in GridFS
    /// - 3 purchases linking users and PDFs
    ///
    /// If any of the collections already contain data,
    /// the seeding process is skipped to avoid duplicates.
    /// </summary>
    public async Task SeedAsync()
    {
        var users = db.GetCollection<User>("users");
        var pdfs = db.GetCollection<PdfDocument>("pdfs");
        var purchases = db.GetCollection<Purchase>("purchases");

        // Check whether the database already contains data
        var hasUsers = await users.Find(_ => true).AnyAsync();
        var hasPdfs = await pdfs.Find(_ => true).AnyAsync();
        var hasPurchases = await purchases.Find(_ => true).AnyAsync();

        if (hasUsers || hasPdfs || hasPurchases)
            return;

        // ----------------------------------------------------
        // 1) Seed users (3 users, including 1 admin)
        // ----------------------------------------------------
        var admin = new User
        {
            UserName = "admin",
            Email = "admin@pdfmarket.local",
            PasswordHash = "Admin123!", // Plain text for project scope (hashed in production)
            Role = "Admin",
            PointsBalance = 999999
        };

        var user1 = new User
        {
            UserName = "alice",
            Email = "alice@pdfmarket.local",
            PasswordHash = "Alice123!",
            Role = "User",
            PointsBalance = 3000000
        };

        var user2 = new User
        {
            UserName = "bob",
            Email = "bob@pdfmarket.local",
            PasswordHash = "Bob123!",
            Role = "User",
            PointsBalance = 3000000
        };

        await users.InsertManyAsync(new[] { admin, user1, user2 });

        // ----------------------------------------------------
        // 2) Seed PDFs and upload files to storage (GridFS)
        // ----------------------------------------------------

        /// <summary>
        /// Creates minimal valid PDF bytes for seed files.
        /// The actual content is not important for functionality testing.
        /// </summary>
        static byte[] FakePdfBytes(string title)
        {
            var text =
                $"%PDF-1.4\n% Fake PDF for seed: {title}\n" +
                "1 0 obj\n<<>>\nendobj\ntrailer\n<<>>\n%%EOF\n";

            return System.Text.Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        /// Uploads a seeded PDF file to file storage and returns its storage ID.
        /// </summary>
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
            Tags = new[] { "architecture", "clean", "notes" },
            IsActive = true,
            FileStorageId = pdf1StorageId
        };

        var pdf2 = new PdfDocument
        {
            Title = "MongoDB Cheat Sheet",
            Description = "Seeded PDF: common queries and patterns.",
            UploaderUserId = user1.Id,
            PriceInPoints = 75,
            Tags = new[] { "mongodb", "database" },
            IsActive = true,
            FileStorageId = pdf2StorageId
        };

        var pdf3 = new PdfDocument
        {
            Title = "JWT Quick Guide",
            Description = "Seeded PDF: JWT basics and roles.",
            UploaderUserId = user2.Id,
            PriceInPoints = 60,
            Tags = new[] { "jwt", "security", "auth" },
            IsActive = true,
            FileStorageId = pdf3StorageId
        };

        await pdfs.InsertManyAsync(new[] { pdf1, pdf2, pdf3 });

        // ----------------------------------------------------
        // 3) Seed purchases and update ownership + points
        // ----------------------------------------------------
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

        await purchases.InsertManyAsync(new[] { purchase1, purchase2, purchase3 });

        // Update owned PDFs and points balance
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
