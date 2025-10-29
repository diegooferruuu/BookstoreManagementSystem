using ServiceUsers.Domain.Models;

namespace ServiceUsers.Domain.Interfaces
{
    public interface ITokenGenerator
    {
        string CreateToken(User user, IEnumerable<string> roles, DateTimeOffset now, Infrastructure.Auth.JwtOptions options);
    }
}
