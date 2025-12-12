using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PdfMarket.Application.Abstractions.Security;

namespace PdfMarket.Infrastructure.Auth;

/// <summary>
/// Generates JWT tokens used for authenticating and authorizing API requests.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <summary>
    /// Creates a signed JWT containing the user's identity and role claims.
    /// </summary>
    /// <remarks>
    /// The token includes standard JWT claims and ASP.NET Core identity claims:
    /// - sub / NameIdentifier: userId
    /// - unique_name / Name: userName
    /// - role: role
    /// </remarks>
    public string GenerateToken(string userId, string userName, string role)
    {
        // Read JWT settings from configuration (appsettings.json / environment).
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing");
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");

        // Build signing key and credentials (HMAC SHA256).
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims are used by ASP.NET Core authorization policies and [Authorize(Roles = "...")].
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.UniqueName, userName),

            // These are the claims ASP.NET Core commonly reads:
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Role, role)
        };

        // Create the token (2 hour lifetime).
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
