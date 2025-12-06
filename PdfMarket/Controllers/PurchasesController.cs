using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfMarket.Application.Services;
using PdfMarket.Contracts.Purchases;

namespace PdfMarket.Controllers;

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

    [HttpPost]
    public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await purchaseService.PurchaseAsync(userId, request);
            // hvis købet lykkes
            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message == "Not enough points")
        {
            // pæn fejl i stedet for 500
            return BadRequest(new { message = ex.Message });
        }
    }
}
