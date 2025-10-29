using Npgsql;
using ServiceUsers.Domain.Interfaces;
using ServiceUsers.Domain.Models;
using ServiceCommon.Domain.Services;

namespace ServiceUsers.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataBase _database;

        public UserRepository(IDataBase database)
        {
            _database = database;
        }

        public async Task<User?> GetByUserOrEmailAsync(string userOrEmail, CancellationToken ct = default)
        {
            var input = (userOrEmail ?? string.Empty).Trim().ToLowerInvariant();
            var sql = "SELECT id, username, email, first_name, last_name, middle_name, password_hash, is_active FROM users WHERE (LOWER(username)=@ue OR LOWER(email)=@ue) LIMIT 1";
            
            await using var conn = _database.GetConnection();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ue", input);
            
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                return new User
                {
                    Id = reader.GetGuid(0),
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

        public async Task<List<string>> GetRolesAsync(Guid userId, CancellationToken ct = default)
        {
            var roles = new List<string>();
            var sql = "SELECT r.name FROM roles r JOIN user_roles ur ON ur.role_id=r.id WHERE ur.user_id=@id";
            
            await using var conn = _database.GetConnection();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", userId);
            
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
                roles.Add(reader.GetString(0));
            
            return roles;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var sql = "SELECT id, username, email, first_name, last_name, middle_name, password_hash, is_active FROM users WHERE id=@id";
            
            await using var conn = _database.GetConnection();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                return new User
                {
                    Id = reader.GetGuid(0),
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

        public async Task<List<User>> GetAllAsync(CancellationToken ct = default)
        {
            var users = new List<User>();
            var sql = "SELECT id, username, email, first_name, last_name, middle_name, password_hash, is_active FROM users WHERE is_active = TRUE ORDER BY id";
            
            await using var conn = _database.GetConnection();
            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            
            while (await reader.ReadAsync(ct))
            {
                users.Add(new User
                {
                    Id = reader.GetGuid(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    FirstName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    LastName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    MiddleName = reader.IsDBNull(5) ? null : reader.GetString(5),
                    PasswordHash = reader.GetString(6),
                    IsActive = reader.GetBoolean(7)
                });
            }
            return users;
        }

        public async Task CreateAsync(User user, string password, List<string> roles, CancellationToken ct = default)
        {
            var sql = @"INSERT INTO users (username, email, first_name, last_name, middle_name, password_hash, is_active) 
                        VALUES (@username, @email, @firstName, @lastName, @middleName, @passwordHash, @isActive)
                        RETURNING id";
            
            await using var conn = _database.GetConnection();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
            cmd.Parameters.AddWithValue("@lastName", user.LastName);
            cmd.Parameters.AddWithValue("@middleName", (object?)user.MiddleName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@isActive", user.IsActive);
            
            var newId = (Guid)(await cmd.ExecuteScalarAsync(ct))!;
            user.Id = newId;

            // Add roles
            foreach (var roleName in roles)
            {
                var insertSql = @"INSERT INTO user_roles(user_id, role_id) 
                                  SELECT @userId, r.id FROM roles r WHERE r.name=@roleName";
                await using var roleCmd = new NpgsqlCommand(insertSql, conn);
                roleCmd.Parameters.AddWithValue("@userId", newId);
                roleCmd.Parameters.AddWithValue("@roleName", roleName);
                await roleCmd.ExecuteNonQueryAsync(ct);
            }
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            var sql = @"UPDATE users SET username=@username, email=@email, first_name=@firstName, 
                        last_name=@lastName, middle_name=@middleName, password_hash=@passwordHash, 
                        is_active=@isActive WHERE id=@id";
            
            await using var conn = _database.GetConnection();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
            cmd.Parameters.AddWithValue("@lastName", user.LastName);
            cmd.Parameters.AddWithValue("@middleName", (object?)user.MiddleName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@isActive", user.IsActive);
            
            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var sql = "UPDATE users SET is_active = FALSE WHERE id=@id";
            
            await using var conn = _database.GetConnection();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
