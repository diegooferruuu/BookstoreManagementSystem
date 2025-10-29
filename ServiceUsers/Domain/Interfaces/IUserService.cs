using ServiceUsers.Domain.Models;
using System.Collections.Generic;

namespace ServiceUsers.Domain.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        User? Read(Guid id);
        void Create(User user);
        void Create(User user, string password, List<string> roles);
        void Update(User user);
        void Delete(Guid id);
        List<string> GetUserRoles(Guid userId);
        void UpdateUserRoles(Guid userId, List<string> roles);
    }
}
