using System.IO;
using PdfMarket.Contracts.Pdfs;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Application.Abstractions.Storage;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Services;

/// <summary>
/// Application service responsible for PDF-related business logic.
/// Handles browsing, uploading, managing visibility, and secure downloading of PDFs.
/// </summary>
public class PdfService : IPdfService
{
    private readonly IPdfRepository pdfRepository;
    private readonly IUserRepository userRepository;
    private readonly IFileStorage fileStorage;
    private readonly IPurchaseService purchaseService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfService"/> class.
    /// </summary>
    public PdfService(
        IPdfRepository pdfRepository,
        IUserRepository userRepository,
        IFileStorage fileStorage,
        IPurchaseService purchaseService)
    {
        this.pdfRepository = pdfRepository;
        this.userRepository = userRepository;
        this.fileStorage = fileStorage;
        this.purchaseService = purchaseService;
    }

    // ------------------------------------------------------------------
    // Public browsing & read operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Returns a list of publicly visible PDFs matching the given filter.
    /// Only active PDFs are included.
    /// </summary>
    public async Task<IReadOnlyCollection<PdfSummaryDto>> BrowseAsync(PdfFilterRequest filter)
    {
        var pdfs = await pdfRepository.BrowseAsync(filter);
        var users = await userRepository.GetAllAsync();

        return pdfs
            .Select(pdf =>
            {
                var uploader = users.FirstOrDefault(u => u.Id == pdf.UploaderUserId);
                var uploaderName = uploader?.UserName ?? "Unknown";

                return new PdfSummaryDto(
                    pdf.Id,
                    pdf.Title,
                    uploaderName,
                    pdf.PriceInPoints,
                    pdf.Tags
                );
            })
            .ToList();
    }

    /// <summary>
    /// Returns detailed information about a specific PDF.
    /// </summary>
    public async Task<PdfDetailsDto?> GetDetailsAsync(string id)
    {
        var pdf = await pdfRepository.GetByIdAsync(id);
        if (pdf is null)
            return null;

        var uploader = await userRepository.GetByIdAsync(pdf.UploaderUserId);
        var uploaderName = uploader?.UserName ?? "Unknown";

        return new PdfDetailsDto(
            pdf.Id,
            pdf.Title,
            pdf.Description,
            uploaderName,
            pdf.PriceInPoints,
            pdf.Tags,
            1,
            pdf.CreatedAt,
            pdf.IsActive
        );
    }

    /// <summary>
    /// Returns all PDFs uploaded by the given user, including inactive ones.
    /// Used for managing own uploads.
    /// </summary>
    public async Task<IReadOnlyCollection<PdfSummaryDto>> GetMyUploadsAsync(string userId)
    {
        var pdfs = await pdfRepository.GetAllByUploaderAsync(userId);

        var me = await userRepository.GetByIdAsync(userId);
        var uploaderName = me?.UserName ?? "Unknown";

        return pdfs
            .Select(pdf => new PdfSummaryDto(
                pdf.Id,
                pdf.Title,
                uploaderName,
                pdf.PriceInPoints,
                pdf.Tags
            ))
            .ToList();
    }

    // ------------------------------------------------------------------
    // Upload & update operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Uploads a new PDF and rewards the uploader with points.
    /// </summary>
    public async Task<UploadPdfResponse> UploadAsync(
        string userId,
        UploadPdfRequest request,
        Stream pdfStream,
        string fileName)
    {
        var storageId = await fileStorage.UploadAsync(
            pdfStream,
            fileName,
            "application/pdf");

        var pdf = new PdfDocument
        {
            Title = request.Title,
            Description = request.Description,
            PriceInPoints = request.PriceInPoints,
            Tags = request.Tags,
            UploaderUserId = userId,
            FileStorageId = storageId
        };

        await pdfRepository.AddAsync(pdf);

        var user = await userRepository.GetByIdAsync(userId)
                   ?? throw new InvalidOperationException("Uploader not found");

        user.PointsBalance += 1;
        await userRepository.UpdateAsync(user);

        var details = await GetDetailsAsync(pdf.Id)
                      ?? throw new InvalidOperationException("Failed to load uploaded PDF");

        return new UploadPdfResponse(
            Pdf: details,
            UploaderPointsBalance: user.PointsBalance
        );
    }

    /// <summary>
    /// Updates metadata and visibility of an existing PDF.
    /// Only the uploader may perform this action.
    /// </summary>
    public async Task<PdfDetailsDto?> UpdateAsync(string userId, string pdfId, UpdatePdfRequest request)
    {
        var pdf = await pdfRepository.GetByIdAsync(pdfId);
        if (pdf is null || pdf.UploaderUserId != userId)
            return null;

        pdf.Title = request.Title;
        pdf.Description = request.Description;
        pdf.PriceInPoints = request.PriceInPoints;
        pdf.Tags = request.Tags;
        pdf.IsActive = request.IsActive;

        await pdfRepository.UpdateAsync(pdf);

        return await GetDetailsAsync(pdfId);
    }

    /// <summary>
    /// Deactivates a PDF so it is no longer visible in public browsing.
    /// </summary>
    public async Task<bool> DeactivateAsync(string userId, string pdfId)
    {
        var pdf = await pdfRepository.GetByIdAsync(pdfId);
        if (pdf is null || pdf.UploaderUserId != userId)
            return false;

        pdf.IsActive = false;
        await pdfRepository.UpdateAsync(pdf);
        return true;
    }

    // ------------------------------------------------------------------
    // Secure file access
    // ------------------------------------------------------------------

    /// <summary>
    /// Returns a PDF file stream for download.
    /// Only the uploader or a user who has purchased the PDF may download it.
    /// </summary>
    public async Task<PdfFileResult?> GetFileForDownloadAsync(string userId, string pdfId)
    {
        var pdf = await pdfRepository.GetByIdAsync(pdfId);
        if (pdf is null || !pdf.IsActive || string.IsNullOrEmpty(pdf.FileStorageId))
            return null;

        var isUploader = pdf.UploaderUserId == userId;

        if (!isUploader)
        {
            var hasPurchased =
                await purchaseService.HasUserPurchasedPdfAsync(userId, pdfId);

            if (!hasPurchased)
                return null;
        }

        var memoryStream = new MemoryStream();
        await fileStorage.DownloadAsync(pdf.FileStorageId, memoryStream);
        memoryStream.Position = 0;

        var safeTitle = string.IsNullOrWhiteSpace(pdf.Title)
            ? "document"
            : pdf.Title;

        return new PdfFileResult(
            memoryStream,
            $"{safeTitle}.pdf",
            "application/pdf"
        );
    }
}
