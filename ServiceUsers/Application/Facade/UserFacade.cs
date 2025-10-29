using Microsoft.AspNetCore.Identity;
using ServiceCommon.Domain.Interfaces;
using ServiceUsers.Application.DTOs;
using ServiceUsers.Domain.Interfaces;
using ServiceUsers.Domain.Models;

namespace ServiceUsers.Application.Facade
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserService _users;
        private readonly IJwtAuthService _auth;
        private readonly IEmailService _email;
        private readonly IPasswordGenerator _pwdGen;
        private readonly IUsernameGenerator _unameGen;

        public UserFacade(
            IUserService users,
            IJwtAuthService auth,
            IEmailService email,
            IPasswordGenerator pwdGen,
            IUsernameGenerator unameGen)
        {
            _users = users;
            _auth = auth;
            _email = email;
            _pwdGen = pwdGen;
            _unameGen = unameGen;
        }

        public async Task<UserReadDto> CreateUserAsync(UserCreateDto dto, CancellationToken ct = default)
        {
            var baseUsername = _unameGen.GenerateUsernameFromEmail(dto.Email);
            var uniqueUsername = _unameGen.EnsureUniqueUsername(
                baseUsername,
                u => _users.GetAll().Any(x => x.Username.Equals(u, StringComparison.OrdinalIgnoreCase)));

            var plainPassword = _pwdGen.GenerateSecurePassword();

            var temp = new User();
            var hasher = new PasswordHasher<User>();
            var hash = hasher.HashPassword(temp, plainPassword);

            var user = new User
            {
                Email = dto.Email.Trim().ToLowerInvariant(),
                Username = uniqueUsername,
                FirstName = string.Empty,
                LastName = string.Empty,
                PasswordHash = hash,
                IsActive = true
            };

            _users.Create(user, plainPassword, new List<string> { dto.Role });

            var html = $"<p>Usuario: <b>{uniqueUsername}</b></p><p>Contraseña: <b>{plainPassword}</b></p>";
            await _email.SendEmailAsync(dto.Email, "Credenciales de acceso", html, ct);

            var roles = _users.GetUserRoles(user.Id);
            return new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = roles.ToArray()
            };
        }

        public async Task<AuthTokenDto?> LoginAsync(AuthRequestDto req, CancellationToken ct = default)
        {
            var result = await _auth.SignInAsync(req, ct);
            return result.IsSuccess ? result.Value : null;
        }

        public Task<IReadOnlyList<UserReadDto>> GetAllAsync(CancellationToken ct = default)
        {
            var list = _users.GetAll()
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Roles = _users.GetUserRoles(u.Id).ToArray()
                })
                .ToList()
                .AsReadOnly();

            return Task.FromResult((IReadOnlyList<UserReadDto>)list);
        }
    }
}
