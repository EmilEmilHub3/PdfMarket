using MongoDB.Driver;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.Mongo;

/// <summary>
/// MongoDB repository for user accounts.
/// </summary>
public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<User> users;

    /// <summary>
    /// Initializes the user collection.
    /// </summary>
    public MongoUserRepository(IMongoDatabase database)
    {
        users = database.GetCollection<User>("users");
    }

    /// <summary>
    /// Returns a user by identifier.
    /// </summary>
    public async Task<User?> GetByIdAsync(string id)
    {
        return await users
            .Find(u => u.Id == id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Returns a user by username or email.
    /// </summary>
    public async Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail)
    {
        return await users
            .Find(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Returns all users in the system.
    /// </summary>
    public async Task<IReadOnlyCollection<User>> GetAllAsync()
    {
        return await users.Find(_ => true).ToListAsync();
    }

    /// <summary>
    /// Inserts a new user.
    /// </summary>
    public async Task AddAsync(User user)
    {
        await users.InsertOneAsync(user);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    public async Task UpdateAsync(User user)
    {
        await users.ReplaceOneAsync(
            u => u.Id == user.Id,
            user,
            new ReplaceOptions { IsUpsert = false });
    }
}
