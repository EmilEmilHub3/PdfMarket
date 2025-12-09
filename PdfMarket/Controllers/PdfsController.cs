using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfMarket.Contracts.Pdfs;
using PdfMarket.Application.Services;
using System.Security.Claims;

namespace PdfMarket.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfsController : ControllerBase
{
    private readonly IPdfService pdfService;

    public PdfsController(IPdfService pdfService)
    {
        this.pdfService = pdfService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Browse([FromQuery] PdfFilterRequest filter)
    {
        var result = await pdfService.BrowseAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(string id)
    {
        var pdf = await pdfService.GetDetailsAsync(id);
        if (pdf is null)
            return NotFound();

        return Ok(pdf);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload([FromForm] UploadPdfRequest request, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Missing PDF file.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await using var stream = file.OpenReadStream();
        var result = await pdfService.UploadAsync(userId, request, stream, file.FileName);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

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

    // NEW: Download endpoint
    [HttpGet("{id}/download")]
    [Authorize]
    public async Task<IActionResult> Download(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var fileResult = await pdfService.GetFileForDownloadAsync(userId, id);
        if (fileResult is null)
            return NotFound();

        return File(fileResult.Stream, fileResult.ContentType, fileResult.FileName);
    }
}
