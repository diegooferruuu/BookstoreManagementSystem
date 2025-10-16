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

            var hasher = new PasswordHasher<object>();
            var adminHash = hasher.HashPassword(null, "admin123456");

            await using (var checkAdminRole = new NpgsqlCommand("SELECT id FROM roles WHERE name='Admin' LIMIT 1;", conn))
            {
                var exists = await checkAdminRole.ExecuteScalarAsync(ct);
                if (exists is null)
                {
                    await using var insertAdmin = new NpgsqlCommand("INSERT INTO roles(name) VALUES('Admin');", conn);
                    await insertAdmin.ExecuteNonQueryAsync(ct);
                }
            }
            await using (var checkEmpRole = new NpgsqlCommand("SELECT id FROM roles WHERE name='Employee' LIMIT 1;", conn))
            {
                var exists = await checkEmpRole.ExecuteScalarAsync(ct);
                if (exists is null)
                {
                    await using var insertEmp = new NpgsqlCommand("INSERT INTO roles(name) VALUES('Employee');", conn);
                    await insertEmp.ExecuteNonQueryAsync(ct);
                }
            }

            // Admin
            var email = "admin@local";
            var username = "admin";
            Guid adminId;
            // Buscar usuario por email
            await using (var findAdmin = new NpgsqlCommand("SELECT id FROM users WHERE email=@e LIMIT 1;", conn))
            {
                findAdmin.Parameters.AddWithValue("@e", email);
                var existing = await findAdmin.ExecuteScalarAsync(ct);
                if (existing is null)
                {
                    // Insertar admin
                    await using var insertAdmin = new NpgsqlCommand(@"INSERT INTO users(username,email,first_name,last_name,password_hash,is_active)
VALUES(@u,@e,'Administrator','',@ph,TRUE)
RETURNING id;", conn);
                    insertAdmin.Parameters.AddWithValue("@u", username);
                    insertAdmin.Parameters.AddWithValue("@e", email);
                    insertAdmin.Parameters.AddWithValue("@ph", adminHash);
                    adminId = (Guid)(await insertAdmin.ExecuteScalarAsync(ct))!;
                }
                else
                {
                    adminId = (Guid)existing;
                    // Actualizar datos y reactivar
                    await using var updateAdmin = new NpgsqlCommand(@"UPDATE users SET
username=@u, first_name='Administrator', last_name='', password_hash=@ph, is_active=TRUE
WHERE id=@id;", conn);
                    updateAdmin.Parameters.AddWithValue("@u", username);
                    updateAdmin.Parameters.AddWithValue("@ph", adminHash);
                    updateAdmin.Parameters.AddWithValue("@id", adminId);
                    await updateAdmin.ExecuteNonQueryAsync(ct);
                }
            }

            // Asegurar asignación del rol Admin al usuario admin
            await using (var ensureAdminRole = new NpgsqlCommand(@"INSERT INTO user_roles(user_id, role_id)
SELECT @uid, r.id FROM roles r
WHERE r.name='Admin'
AND NOT EXISTS (
    SELECT 1 FROM user_roles ur WHERE ur.user_id=@uid AND ur.role_id=r.id
);", conn))
            {
                ensureAdminRole.Parameters.AddWithValue("@uid", adminId);
                await ensureAdminRole.ExecuteNonQueryAsync(ct);
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
