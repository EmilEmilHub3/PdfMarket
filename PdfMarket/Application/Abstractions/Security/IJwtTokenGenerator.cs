namespace PdfMarket.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string userName, string role);
}
