using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUserOrEmailAsync(string userOrEmail, CancellationToken ct = default);
        Task<List<string>> GetRolesAsync(int userId, CancellationToken ct = default);
        Task SeedAdminAndRolesAsync(string adminEmail, string adminPasswordHash, CancellationToken ct = default);
    }
}
