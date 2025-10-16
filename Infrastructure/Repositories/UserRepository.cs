using System.Text.RegularExpressions;
using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Infrastructure.DataBase;
using Npgsql;

namespace BookstoreManagementSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _conn;
        public UserRepository()
        {
            _conn = DataBaseConnection.Instance.GetConnection();
        }

        public async Task<User?> GetByUserOrEmailAsync(string userOrEmail, CancellationToken ct = default)
        {
            var input = (userOrEmail ?? string.Empty).Trim().ToLowerInvariant();
            var sql = "SELECT id, username, email, first_name, last_name, middle_name, password_hash, is_active FROM users WHERE (LOWER(username)=@ue OR LOWER(email)=@ue) LIMIT 1";
            await using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@ue", input);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    FirstName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    LastName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    MiddleName = reader.IsDBNull(5) ? null : reader.GetString(5),
                    PasswordHash = reader.GetString(6),
                    IsActive = reader.GetBoolean(7)
                };
            }
            return null;
        }

        public async Task<List<string>> GetRolesAsync(int userId, CancellationToken ct = default)
        {
            var roles = new List<string>();
            var sql = "SELECT r.name FROM roles r JOIN user_roles ur ON ur.role_id=r.id WHERE ur.user_id=@id";
            await using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", userId);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
                roles.Add(reader.GetString(0));
            return roles;
        }


        public async Task SeedAdminAndRolesAsync(string adminEmail, string adminPasswordHash, CancellationToken ct = default)
        {
            await EnsureRoleExistsAsync("Admin", ct);
            await EnsureRoleExistsAsync("Client", ct);

            var email = adminEmail.Trim().ToLowerInvariant();
            var username = email.Split('@')[0];
            var sql = "SELECT id FROM users WHERE email=@e LIMIT 1";
            await using (var check = new NpgsqlCommand(sql, _conn))
            {
                check.Parameters.AddWithValue("@e", email);
                var existing = await check.ExecuteScalarAsync(ct);
                if (existing is null)
                {
                    var insert = new NpgsqlCommand("INSERT INTO users (username,email,full_name,password_hash,is_active) VALUES (@u,@e,@fn,@ph,TRUE) RETURNING id", _conn);
                    insert.Parameters.AddWithValue("@u", username);
                    insert.Parameters.AddWithValue("@e", email);
                    insert.Parameters.AddWithValue("@fn", "Administrator");
                    insert.Parameters.AddWithValue("@ph", adminPasswordHash);
                    var newId = (int)(await insert.ExecuteScalarAsync(ct))!;
                    await AddUserRoleAsync(newId, "Admin", ct);
                }
                else
                {
                    await AddUserRoleByEmailAsync(email, "Admin", ct);
                }
            }
        }

        // Eliminado: backfill y creación de usuarios desde clientes

        private async Task EnsureRoleExistsAsync(string roleName, CancellationToken ct)
        {
            var sql = "INSERT INTO roles(name) VALUES(@n) ON CONFLICT(name) DO NOTHING";
            await using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@n", roleName);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private async Task AddUserRoleAsync(int userId, string roleName, CancellationToken ct)
        {
            var sql = "INSERT INTO user_roles(user_id, role_id) SELECT @u, r.id FROM roles r WHERE r.name=@n ON CONFLICT DO NOTHING";
            await using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@n", roleName);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private async Task AddUserRoleByEmailAsync(string email, string roleName, CancellationToken ct)
        {
            var sql = @"INSERT INTO user_roles(user_id, role_id)
SELECT u.id, r.id FROM users u, roles r WHERE u.email=@e AND r.name=@n
ON CONFLICT DO NOTHING";
            await using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@n", roleName);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        // Nota: No se generan sufijos; se espera unicidad del local-part por datos.
    }
}
