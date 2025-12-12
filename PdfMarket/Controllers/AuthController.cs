using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfMarket.Application.Services;
using PdfMarket.Contracts.Auth;

namespace PdfMarket.Controllers;

/// <summary>
/// Authentication endpoints for registering and logging in.
/// </summary>
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

    /// <summary>
    /// Registers a new user and returns an auth response (including JWT).
    /// </summary>
    /// <remarks>
    /// Returns 409 Conflict if the user already exists.
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await authService.RegisterAsync(request);

            // 201 Created is appropriate for new resources.
            return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
        }
        catch (InvalidOperationException ex) when (ex.Message == "User already exists")
        {
            // Conflict is the correct response for duplicate user registration.
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Logs in a user and returns an auth response (including JWT).
    /// </summary>
    /// <remarks>
    /// Returns 401 Unauthorized if credentials are invalid.
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        if (result is null)
            return Unauthorized();

        return Ok(result);
    }
}
