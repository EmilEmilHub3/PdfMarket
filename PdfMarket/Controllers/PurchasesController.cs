using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfMarket.Contracts.Purchases;
using PdfMarket.Application.Services;
using System.Security.Claims;

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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "dummy-user";

        var result = await purchaseService.PurchaseAsync(userId, request);
        if (result is null)
            return BadRequest();

        return CreatedAtAction(nameof(Purchase), new { id = result.PurchaseId }, result);
    }
}
