using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface ITokenGenerator
    {
        string CreateToken(User user, IEnumerable<string> roles, DateTimeOffset now, BookstoreManagementSystem.Infrastructure.Auth.JwtOptions options);
    }
}
