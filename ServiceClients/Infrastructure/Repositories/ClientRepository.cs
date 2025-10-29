using Npgsql;
using ServiceClients.Domain.Interfaces;
using ServiceClients.Domain.Models;
using ServiceCommon.Domain.Services;

namespace ServiceClients.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDataBase _database;

        public ClientRepository(IDataBase database)
        {
            _database = database;
        }

        public List<Client> GetAll()
        {
            var clients = new List<Client>();
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand("SELECT * FROM clients WHERE is_active = TRUE ORDER BY last_name, first_name, middle_name", conn);
            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                clients.Add(new Client
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                    MiddleName = reader.IsDBNull(reader.GetOrdinal("middle_name")) ? null : reader.GetString(reader.GetOrdinal("middle_name")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                    Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                    Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }

            return clients;
        }

        public Client? Read(Guid id)
        {
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand("SELECT * FROM clients WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Client
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                    MiddleName = reader.IsDBNull(reader.GetOrdinal("middle_name")) ? null : reader.GetString(reader.GetOrdinal("middle_name")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                    Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                    Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }

            return null;
        }

        public void Create(Client client)
        {
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO clients (first_name, last_name, middle_name, email, phone, address)
                VALUES (@first_name, @last_name, @middle_name, @email, @phone, @address)", conn);

            cmd.Parameters.AddWithValue("@first_name", client.FirstName);
            cmd.Parameters.AddWithValue("@last_name", client.LastName);
            cmd.Parameters.AddWithValue("@middle_name", client.MiddleName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", client.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", client.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", client.Address ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public void Update(Client client)
        {
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand(@"
                UPDATE clients SET 
                    first_name = @first_name,
                    last_name = @last_name,
                    middle_name = @middle_name,
                    email = @email,
                    phone = @phone,
                    address = @address
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@id", client.Id);
            cmd.Parameters.AddWithValue("@first_name", client.FirstName);
            cmd.Parameters.AddWithValue("@last_name", client.LastName);
            cmd.Parameters.AddWithValue("@middle_name", client.MiddleName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", client.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", client.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", client.Address ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public void Delete(Guid id)
        {
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand("UPDATE clients SET is_active = FALSE WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
            cmd.ExecuteNonQuery();
        }
    }
}
