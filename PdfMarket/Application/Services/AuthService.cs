using PdfMarket.Contracts.Auth;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Application.Abstractions.Security;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Services;

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
        var existing = await userRepository.GetByUserNameOrEmailAsync(request.UserName);
        if (existing is not null)
            throw new InvalidOperationException("User already exists");

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = request.Password, // TODO hash this later
            Role = "User",
            PointsBalance = 0
        };

        await userRepository.AddAsync(user);

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
