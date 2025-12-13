using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfMarket.Application.Services;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.Controllers;

/// <summary>
/// Admin-only endpoints for moderation, user overview, and platform statistics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService adminService;

    public AdminController(IAdminService adminService)
    {
        this.adminService = adminService;
    }

    /// <summary>
    /// Returns an overview of all users (admin only).
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await adminService.GetUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Returns platform-wide statistics (admin only).
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await adminService.GetStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Returns a list of all PDFs (active and inactive) for moderation (admin only).
    /// </summary>
    [HttpGet("pdfs")]
    public async Task<IActionResult> GetPdfs()
    {
        var pdfs = await adminService.GetPdfsAsync();
        return Ok(pdfs);
    }

    /// <summary>
    /// Deletes a PDF and its stored file (admin only).
    /// </summary>
    [HttpDelete("pdfs/{pdfId}")]
    public async Task<IActionResult> DeletePdf(string pdfId)
    {
        var ok = await adminService.DeletePdfAsync(pdfId);
        if (!ok)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Resets a user's password (admin only).
    /// </summary>
    [HttpPost("users/{userId}/reset-password")]
    public async Task<IActionResult> ResetPassword(string userId, [FromBody] ResetPasswordRequest request)
    {
        // Validate payload before calling the service.
        if (request is null || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest("NewPassword is required.");

        var ok = await adminService.ResetUserPasswordAsync(userId, request.NewPassword);
        if (!ok)
            return NotFound();

        return NoContent();
    }

    [HttpPut("users/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequest request)
    {
        if (request is null)
            return BadRequest("Request body is required.");

        var ok = await adminService.UpdateUserAsync(userId, request);
        if (!ok)
            return NotFound();

        return NoContent();
    }



}
