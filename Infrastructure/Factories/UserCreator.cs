using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;

namespace BookstoreManagementSystem.Infrastructure.Factories
{
    public class UserCreator
    {
        public UserService FactoryMethod()
        {
            return new UserService(new UserRepository());
        }
    }
}
