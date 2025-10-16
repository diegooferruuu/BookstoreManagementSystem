using Npgsql;
using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Infrastructure.DataBase;
using System.Collections.Generic;

namespace BookstoreManagementSystem.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
         private readonly NpgsqlConnection _connection;

        public ClientRepository()
        {
            _connection = DataBaseConnection.Instance.GetConnection();
        }

        public void Create(Client client)
        {
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO clients (first_name, last_name, middle_name, email, phone, address)
                VALUES (@first_name, @last_name, @middle_name, @email, @phone, @address)", _connection);


            cmd.Parameters.AddWithValue("@first_name", client.FirstName);
            cmd.Parameters.AddWithValue("@last_name", client.LastName);
            cmd.Parameters.AddWithValue("@middle_name", client.MiddleName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", client.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", client.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", client.Address ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();

        }

    public Client? Read(int id)
        {
            using var cmd = new NpgsqlCommand("SELECT * FROM clients WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Client
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
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

        public void Update(Client client)
        {
            using var cmd = new NpgsqlCommand(@"
                UPDATE clients SET 
                    first_name = @first_name,
                    last_name = @last_name,
                    middle_name = @middle_name,
                    email = @email,
                    phone = @phone,
                    address = @address
                WHERE id = @id", _connection);

            cmd.Parameters.AddWithValue("@id", client.Id);
            cmd.Parameters.AddWithValue("@first_name", client.FirstName);
            cmd.Parameters.AddWithValue("@last_name", client.LastName);
            cmd.Parameters.AddWithValue("@middle_name", client.MiddleName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", client.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", client.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", client.Address ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();

        }

        public void Delete(int id)
        {
            using var cmd = new NpgsqlCommand("UPDATE clients SET is_active = FALSE WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Client> GetAll()
        {
            var clients = new List<Client>();
            using var cmd = new NpgsqlCommand("SELECT * FROM clients WHERE is_active = TRUE  ORDER BY last_name, first_name, middle_name", _connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                clients.Add(new Client
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
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
    }
}

