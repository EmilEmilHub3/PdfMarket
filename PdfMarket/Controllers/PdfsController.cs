using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfMarket.Application.Services;
using PdfMarket.Contracts.Pdfs;

namespace PdfMarket.Controllers;

/// <summary>
/// Public and authenticated endpoints for browsing, managing, and downloading PDFs.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PdfsController : ControllerBase
{
    private readonly IPdfService pdfService;

    public PdfsController(IPdfService pdfService)
    {
        this.pdfService = pdfService;
    }

    // ------------------------------------------------------------
    // Public endpoints (anonymous access allowed)
    // ------------------------------------------------------------

    /// <summary>
    /// Returns a list of active PDFs matching the filter.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Browse([FromQuery] PdfFilterRequest filter)
    {
        var result = await pdfService.BrowseAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Returns details about a specific PDF.
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(string id)
    {
        var pdf = await pdfService.GetDetailsAsync(id);
        if (pdf is null)
            return NotFound();

        return Ok(pdf);
    }

    // ------------------------------------------------------------
    // Authenticated endpoints
    // ------------------------------------------------------------

    /// <summary>
    /// Uploads a PDF and returns 201 Created + updated uploader points balance.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload([FromForm] UploadPdfRequest request, IFormFile file)
    {
        // Validate input early to avoid unnecessary work.
        if (file is null || file.Length == 0)
            return BadRequest("Missing PDF file.");

        // UserId comes from the JWT (ClaimTypes.NameIdentifier).
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await using var stream = file.OpenReadStream();

        // Service handles: store file, create PDF, reward points.
        var result = await pdfService.UploadAsync(userId, request, stream, file.FileName);

        // 201 Created + Location header pointing to GET /api/pdfs/{id}.
        return CreatedAtAction(nameof(GetById), new { id = result.Pdf.Id }, result);
    }

    /// <summary>
    /// Updates metadata and visibility for a PDF. Only the uploader can update.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(string id, [FromBody] UpdatePdfRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var updated = await pdfService.UpdateAsync(userId, id, request);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    /// <summary>
    /// Deactivates a PDF (hides it from public browsing). Only the uploader can deactivate.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Deactivate(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var success = await pdfService.DeactivateAsync(userId, id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Downloads a PDF if the user is allowed (uploader or purchaser).
    /// </summary>
    /// <remarks>
    /// Returns:
    /// - 401 if not authenticated
    /// - 404 if the PDF doesn't exist or is inactive
    /// - 403 if the PDF exists but the user is not allowed to download it
    /// </remarks>
    [HttpGet("{id}/download")]
    [Authorize]
    public async Task<IActionResult> Download(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var fileResult = await pdfService.GetFileForDownloadAsync(userId, id);

        if (fileResult is null)
        {
            // Distinguish "not found / inactive" from "not allowed".
            var pdf = await pdfService.GetDetailsAsync(id);
            if (pdf is null || !pdf.IsActive)
                return NotFound();

            // Resource exists and is active, but user is not authorized to download.
            return Forbid();
        }

        return File(fileResult.Stream, fileResult.ContentType, fileResult.FileName);
    }

    /// <summary>
    /// Returns all PDFs uploaded by the current user, including inactive ones (so they can re-enable them).
    /// </summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyUploads()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await pdfService.GetMyUploadsAsync(userId);
        return Ok(result);
    }
}
