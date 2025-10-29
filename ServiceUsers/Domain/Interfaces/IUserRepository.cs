using ServiceUsers.Domain.Models;

namespace ServiceUsers.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUserOrEmailAsync(string userOrEmail, CancellationToken ct = default);
        Task<List<string>> GetRolesAsync(Guid userId, CancellationToken ct = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<User>> GetAllAsync(CancellationToken ct = default);
        Task CreateAsync(User user, string password, List<string> roles, CancellationToken ct = default);
        Task UpdateAsync(User user, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
