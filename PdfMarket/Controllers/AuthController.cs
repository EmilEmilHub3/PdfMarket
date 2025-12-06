using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfMarket.Application.Services;
using PdfMarket.Contracts.Auth;

namespace PdfMarket.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
        }
        catch (InvalidOperationException ex) when (ex.Message == "User already exists")
        {
            // 409 Conflict når brugeren allerede findes
            return Conflict(new { message = ex.Message });
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        if (result is null)
            return Unauthorized();

        return Ok(result);
    }
}
