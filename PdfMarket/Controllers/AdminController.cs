using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfMarket.Application.Services;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.Controllers;

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

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await adminService.GetUsersAsync();
        return Ok(users);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await adminService.GetStatsAsync();
        return Ok(stats);
    }

    [HttpGet("pdfs")]
    public async Task<IActionResult> GetPdfs()
    {
        var pdfs = await adminService.GetPdfsAsync();
        return Ok(pdfs);
    }

    [HttpDelete("pdfs/{pdfId}")]
    public async Task<IActionResult> DeletePdf(string pdfId)
    {
        var ok = await adminService.DeletePdfAsync(pdfId);
        if (!ok) return NotFound();
        return NoContent();
    }

    // NEW: reset password
    [HttpPost("users/{userId}/reset-password")]
    public async Task<IActionResult> ResetPassword(string userId, [FromBody] ResetPasswordRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest("NewPassword is required.");

        var ok = await adminService.ResetUserPasswordAsync(userId, request.NewPassword);
        if (!ok) return NotFound();

        return NoContent();
    }
}
