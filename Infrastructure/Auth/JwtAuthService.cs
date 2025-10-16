using BookstoreManagementSystem.Application.DTOs;
using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace BookstoreManagementSystem.Infrastructure.Auth
{
    public class JwtAuthService : IJwtAuthService
    {
        private readonly IUserRepository _users;
        private readonly ITokenGenerator _tokens;
        private readonly JwtOptions _options;
        private readonly PasswordHasher<object> _hasher = new();

        public JwtAuthService(IUserRepository users, ITokenGenerator tokens, JwtOptions options)
        {
            _users = users; _tokens = tokens; _options = options;
        }

        public async Task<Result<AuthTokenDto>> SignInAsync(AuthRequestDto req, CancellationToken ct = default)
        {
            var input = (req.UserOrEmail ?? string.Empty).Trim().ToLowerInvariant();
            var user = await _users.GetByUserOrEmailAsync(input, ct);
            if (user is null || !user.IsActive)
                return Result<AuthTokenDto>.Fail("Credenciales inválidas.");

            var verify = _hasher.VerifyHashedPassword(null, user.PasswordHash, req.Password ?? string.Empty);
            if (verify == PasswordVerificationResult.Failed)
                return Result<AuthTokenDto>.Fail("Credenciales inválidas.");

            var roles = await _users.GetRolesAsync(user.Id, ct);
            var now = DateTimeOffset.UtcNow;
            var jwt = _tokens.CreateToken(user, roles, now, _options);

            return Result<AuthTokenDto>.Ok(new AuthTokenDto
            {
                AccessToken = jwt,
                ExpiresAt = now.AddMinutes(_options.ExpiresMinutes),
                UserName = user.Username,
                Roles = roles.ToArray(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName
            });
        }
    }
}
