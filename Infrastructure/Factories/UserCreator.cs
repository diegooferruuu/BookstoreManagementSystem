using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Infrastructure.Repositories;

namespace BookstoreManagementSystem.Infrastructure.Factories
{
    public class UserCreator
    {

        private readonly IUserRepository _userRepository;

        public UserCreator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IUserService FactoryMethod()
        {
            return new UserService(_userRepository);
        }
    }
}
