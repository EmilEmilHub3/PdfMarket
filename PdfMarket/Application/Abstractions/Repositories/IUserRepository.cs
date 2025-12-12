using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Abstractions.Repositories;

/// <summary>
/// Repository abstraction for user persistence.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Returns a user by identifier.
    /// </summary>
    Task<User?> GetByIdAsync(string id);

    /// <summary>
    /// Returns a user by username or email.
    /// Used for login and registration validation.
    /// </summary>
    Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail);

    /// <summary>
    /// Returns all users in the system.
    /// Used by admin functionality.
    /// </summary>
    Task<IReadOnlyCollection<User>> GetAllAsync();

    /// <summary>
    /// Inserts a new user.
    /// </summary>
    Task AddAsync(User user);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    Task UpdateAsync(User user);
}
