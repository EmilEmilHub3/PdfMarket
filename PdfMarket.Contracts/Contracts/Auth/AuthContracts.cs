namespace PdfMarket.Contracts.Auth;

/// <summary>
/// Request payload for registering a new user.
/// </summary>
public record RegisterRequest(
    string UserName,
    string Email,
    string Password
);

/// <summary>
/// Request payload for login. Supports either username or email.
/// </summary>
public record LoginRequest(
    string UserNameOrEmail,
    string Password
);

/// <summary>
/// Response returned after successful authentication.
/// </summary>
public record AuthResponse(
    string UserId,
    string UserName,
    string Role,
    int PointsBalance,
    string Token
);
