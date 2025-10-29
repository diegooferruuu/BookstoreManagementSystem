using ServiceUsers.Domain.Interfaces;
using ServiceUsers.Domain.Models;

namespace ServiceUsers.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public List<User> GetAll() => _repository.GetAllAsync().Result;
        public User? Read(Guid id) => _repository.GetByIdAsync(id).Result;
        
        public void Create(User user)
        {
            // CreateAsync requiere password y roles, usando valores por defecto temporalmente
            _repository.CreateAsync(user, "temporal", new List<string> { "User" }).Wait();
        }
        
        public void Update(User user) => _repository.UpdateAsync(user).Wait();
        public void Delete(Guid id) => _repository.DeleteAsync(id).Wait();
        
        public List<string> GetUserRoles(Guid userId) => _repository.GetRolesAsync(userId).Result;
        
        public void UpdateUserRoles(Guid userId, List<string> roles)
        {
            // Este método no existe en el repository, necesitamos implementarlo
            throw new NotImplementedException("UpdateUserRoles no está implementado en el repository");
        }
    }
}
