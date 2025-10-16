using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Domain.Interfaces
{
    public interface ITokenGenerator
    {
        string CreateToken(User user, IEnumerable<string> roles, DateTimeOffset now, Infrastructure.Auth.JwtOptions options);
    }
}
