using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfMarket.Application.Services;
using PdfMarket.Contracts.Purchases;

namespace PdfMarket.Controllers;

/// <summary>
/// Endpoints for purchasing PDFs and viewing purchase history.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        this.purchaseService = purchaseService;
    }

    /// <summary>
    /// Purchases a PDF for the current user.
    /// </summary>
    /// <remarks>
    /// Returns:
    /// - 401 if not authenticated
    /// - 404 if the PDF does not exist or is inactive
    /// - 400 if business rules fail (e.g., insufficient points)
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await purchaseService.PurchaseAsync(userId, request);
            if (result is null)
                return NotFound();

            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message == "Not enough points")
        {
            // Returning 400 keeps failures explicit and avoids a generic 500.
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Returns the current user's purchase history.
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyPurchases()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await purchaseService.GetMyPurchasesAsync(userId);
        return Ok(result);
    }
}
