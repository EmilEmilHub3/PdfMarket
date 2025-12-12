using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Application.Abstractions.Security;
using PdfMarket.Contracts.Auth;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Services;

/// <summary>
/// Implements register/login and JWT generation.
/// Note: password hashing is intentionally simplified in this project scope.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly IJwtTokenGenerator jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        this.userRepository = userRepository;
        this.jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Ensure username/email is unique to keep auth stable and predictable.
        var existing = await userRepository.GetByUserNameOrEmailAsync(request.UserName);
        if (existing is not null)
            throw new InvalidOperationException("User already exists");

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,

            // Project scope: stored as-is. In production, store a salted hash.
            PasswordHash = request.Password,

            Role = "User",

            // Project scope: starting balance for testing/easy demos.
            PointsBalance = 100000
        };

        await userRepository.AddAsync(user);

        // JWT contains claims used by the API for authorization decisions.
        var token = jwtTokenGenerator.GenerateToken(user.Id, user.UserName, user.Role);

        return new AuthResponse(
            user.Id,
            user.UserName,
            user.Role,
            user.PointsBalance,
            token
        );
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByUserNameOrEmailAsync(request.UserNameOrEmail);
        if (user is null)
            return null;

        // Project scope: plaintext compare. In production, verify with a password hasher.
        if (user.PasswordHash != request.Password)
            return null;

        var token = jwtTokenGenerator.GenerateToken(user.Id, user.UserName, user.Role);

        return new AuthResponse(
            user.Id,
            user.UserName,
            user.Role,
            user.PointsBalance,
            token
        );
    }
}
