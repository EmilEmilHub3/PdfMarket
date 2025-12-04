using PdfMarket.Domain.Entities;

namespace PdfMarket.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail);
    Task<IReadOnlyCollection<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
