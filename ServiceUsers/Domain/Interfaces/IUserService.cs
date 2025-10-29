using ServiceUsers.Domain.Models;

namespace ServiceUsers.Domain.Interfaces
{
    public interface IUserService
    {
        public List<User> GetAll();
        public User? Read(Guid id);
        public void Create(User user);
        public void Update(User user);
        public void Delete(Guid id);
        public List<string> GetUserRoles(Guid userId);
        public void UpdateUserRoles(Guid userId, List<string> roles);
    }
}
