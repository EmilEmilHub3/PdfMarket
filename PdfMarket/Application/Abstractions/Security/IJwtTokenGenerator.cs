namespace PdfMarket.Application.Abstractions.Security;

/// <summary>
/// Generates JWT tokens used for authentication and authorization.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a signed JWT containing identity and role claims.
    /// </summary>
    string GenerateToken(string userId, string userName, string role);
}
