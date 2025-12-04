using PdfMarket.Contracts.Pdfs;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Application.Abstractions.Storage;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Services;

public class PdfService : IPdfService
{
    private readonly IPdfRepository pdfRepository;
    private readonly IUserRepository userRepository;
    private readonly IFileStorage fileStorage;

    public PdfService(
        IPdfRepository pdfRepository,
        IUserRepository userRepository,
        IFileStorage fileStorage)
    {
        this.pdfRepository = pdfRepository;
        this.userRepository = userRepository;
        this.fileStorage = fileStorage;
    }

    public async Task<IReadOnlyCollection<PdfSummaryDto>> BrowseAsync(PdfFilterRequest filter)
    {
        var pdfs = await pdfRepository.BrowseAsync(filter);
        var users = await userRepository.GetAllAsync();

        var list = pdfs
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

        return list;
    }

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
            1, // fx 1 point per upload
            pdf.CreatedAt,
            pdf.IsActive
        );
    }

    public async Task<PdfDetailsDto> UploadAsync(string userId, UploadPdfRequest request, Stream pdfStream, string fileName)
    {
        // 1) Upload selve filen til GridFS via FileStorage
        var storageId = await fileStorage.UploadAsync(pdfStream, fileName, "application/pdf");

        // 2) Gem metadata i pdfs-collection
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

        // 3) Giv upload-point til brugeren
        var user = await userRepository.GetByIdAsync(userId)
                   ?? throw new InvalidOperationException("Uploader not found");

        user.PointsBalance += 1; // 1 point per upload
        await userRepository.UpdateAsync(user);

        var details = await GetDetailsAsync(pdf.Id);
        return details ?? throw new InvalidOperationException("Failed to load uploaded PDF");
    }

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

    public async Task<bool> DeactivateAsync(string userId, string pdfId)
    {
        var pdf = await pdfRepository.GetByIdAsync(pdfId);
        if (pdf is null || pdf.UploaderUserId != userId)
            return false;

        pdf.IsActive = false;
        await pdfRepository.UpdateAsync(pdf);
        return true;
    }
}
