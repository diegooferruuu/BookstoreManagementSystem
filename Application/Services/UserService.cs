using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public List<User> GetAll() => _repository.GetAll();
        public User? Read(Guid id) => _repository.Read(id);
        public void Create(User user) => _repository.Create(user);
        public void Update(User user) => _repository.Update(user);
        public void Delete(Guid id) => _repository.Delete(id);
        public List<string> GetUserRoles(Guid userId) => _repository.GetUserRoles(userId);
        public void UpdateUserRoles(Guid userId, List<string> roles) => _repository.UpdateUserRoles(userId, roles);
    }
}
