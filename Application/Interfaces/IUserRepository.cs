using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;

namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface IUserRepository : IDataBase<User>
    {
        Task<User?> GetByUserOrEmailAsync(string userOrEmail, CancellationToken ct = default);
        Task<List<string>> GetRolesAsync(Guid userId, CancellationToken ct = default);
        Task SeedAdminAndRolesAsync(string adminEmail, string adminPasswordHash, CancellationToken ct = default);
        
        // Additional methods specific to User
        List<string> GetUserRoles(Guid userId);
        void UpdateUserRoles(Guid userId, List<string> roles);
    }
}
