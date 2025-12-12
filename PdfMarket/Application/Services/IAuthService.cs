using PdfMarket.Contracts.Auth;

namespace PdfMarket.Application.Services;

/// <summary>
/// Application service for authentication and account creation.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and returns an auth response containing a JWT.
    /// Throws if the user already exists.
    /// </summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Attempts login and returns an auth response containing a JWT, or null if credentials are invalid.
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
