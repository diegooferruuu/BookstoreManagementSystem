using ServiceUsers.Domain.Interfaces;
using ServiceUsers.Domain.Models;
using System;
using System.Collections.Generic;

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
            Create(user, "temporal", new List<string> { "User" });
        }

        public void Create(User user, string password, List<string> roles)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("Email requerido", nameof(user));
            if (string.IsNullOrWhiteSpace(user.Username))
                user.Username = user.Email.Split('@')[0].ToLowerInvariant();
            roles ??= new List<string>();
            _repository.CreateAsync(user, password, roles).Wait();
        }

        public void Update(User user) => _repository.UpdateAsync(user).Wait();
        public void Delete(Guid id) => _repository.DeleteAsync(id).Wait();
        public List<string> GetUserRoles(Guid userId) => _repository.GetRolesAsync(userId).Result;
        public void UpdateUserRoles(Guid userId, List<string> roles)
        {
            throw new NotImplementedException();
        }
    }
}
