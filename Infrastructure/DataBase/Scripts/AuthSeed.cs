using BookstoreManagementSystem.Infrastructure.DataBase;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace BookstoreManagementSystem.Infrastructure.DataBase.Scripts
{
    public static class AuthSeed
    {
        public static async Task EnsureAuthSeedAsync(CancellationToken ct = default)
        {
            var conn = DataBaseConnection.Instance.GetConnection();
            // Schema creation removed: DB schema is managed externally now.

            var hasher = new PasswordHasher<object>();
            var adminHash = hasher.HashPassword(null, "admin123456");

            // Roles: Admin y Employee
            await using (var addRole = new NpgsqlCommand("INSERT INTO roles(name) VALUES('Admin') ON CONFLICT DO NOTHING;", conn))
                await addRole.ExecuteNonQueryAsync(ct);
            await using (var addRole2 = new NpgsqlCommand("INSERT INTO roles(name) VALUES('Employee') ON CONFLICT DO NOTHING;", conn))
                await addRole2.ExecuteNonQueryAsync(ct);

            // Admin
            var email = "admin@local";
            var username = "admin";
            await using (var upsert = new NpgsqlCommand(@"INSERT INTO users(username,email,first_name,last_name,password_hash,is_active)
VALUES(@u,@e,'Administrator','',@ph,TRUE)
ON CONFLICT(email) DO UPDATE SET username=EXCLUDED.username, first_name=EXCLUDED.first_name, last_name=EXCLUDED.last_name, password_hash=EXCLUDED.password_hash, is_active=TRUE
RETURNING id;", conn))
            {
                upsert.Parameters.AddWithValue("@u", username);
                upsert.Parameters.AddWithValue("@e", email);
                upsert.Parameters.AddWithValue("@ph", adminHash);
                var adminId = (int)(await upsert.ExecuteScalarAsync(ct))!;

                await using var addAdminRole = new NpgsqlCommand(@"INSERT INTO user_roles(user_id, role_id)
SELECT @uid, r.id FROM roles r WHERE r.name='Admin'
ON CONFLICT DO NOTHING;", conn);
                addAdminRole.Parameters.AddWithValue("@uid", adminId);
                await addAdminRole.ExecuteNonQueryAsync(ct);
            }

            // Asignar rol Employee a todos los usuarios activos que no sean admin y no lo tengan aún
            await using (var assignEmployees = new NpgsqlCommand(@"INSERT INTO user_roles(user_id, role_id)
SELECT u.id, r.id
FROM users u
JOIN roles r ON r.name='Employee'
LEFT JOIN user_roles ur ON ur.user_id = u.id AND ur.role_id = r.id
LEFT JOIN user_roles ura ON ura.user_id = u.id
LEFT JOIN roles ra ON ura.role_id = ra.id AND ra.name = 'Admin'
WHERE ur.user_id IS NULL
  AND (u.email IS DISTINCT FROM 'admin@local')
  AND ra.id IS NULL
  AND u.is_active = TRUE;", conn))
            {
                await assignEmployees.ExecuteNonQueryAsync(ct);
            }
        }
    }
}
