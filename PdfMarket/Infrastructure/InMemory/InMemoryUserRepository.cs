using PdfMarket.Application.Abstractions.Repositories;
using PdfMarket.Domain.Entities;

namespace PdfMarket.Infrastructure.InMemory;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> users = new();

    public Task<User?> GetByIdAsync(string id) =>
        Task.FromResult(users.FirstOrDefault(u => u.Id == id));

    public Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail) =>
        Task.FromResult(
            users.FirstOrDefault(u =>
                u.UserName.Equals(userNameOrEmail, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Equals(userNameOrEmail, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyCollection<User>> GetAllAsync() =>
        Task.FromResult((IReadOnlyCollection<User>)users.ToList());

    public Task AddAsync(User user)
    {
        users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user)
    {
        // In-memory: object is already in the list (by reference),
        // so there's nothing special to do here.
        return Task.CompletedTask;
    }
}
