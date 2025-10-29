using ServiceCommon.Domain.Results;
using ServiceUsers.Application.DTOs;

namespace ServiceUsers.Domain.Interfaces
{
    public interface IJwtAuthService
    {
        Task<Result<AuthTokenDto>> SignInAsync(AuthRequestDto req, CancellationToken ct = default);
    }
}
