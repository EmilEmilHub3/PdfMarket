using MongoDB.Driver;
using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.Mongo;

public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<User> users;

    public MongoUserRepository(IMongoDatabase database)
    {
        users = database.GetCollection<User>("users");
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await users
            .Find(u => u.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail)
    {
        return await users
            .Find(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync()
    {
        var list = await users.Find(_ => true).ToListAsync();
        return list;
    }

    public async Task AddAsync(User user)
    {
        await users.InsertOneAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        await users.ReplaceOneAsync(
            filter: u => u.Id == user.Id,
            replacement: user,
            options: new ReplaceOptions { IsUpsert = false });
    }
}
