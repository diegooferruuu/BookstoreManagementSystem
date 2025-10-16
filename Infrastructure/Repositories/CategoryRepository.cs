using Npgsql;
using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;
using BookstoreManagementSystem.Infrastructure.DataBase;
using BookstoreManagementSystem.Domain.Interfaces;

namespace BookstoreManagementSystem.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly NpgsqlConnection _connection;

        public CategoryRepository()
        {
            _connection = DataBaseConnection.Instance.GetConnection();
        }

        public void Create(Category entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }


        public Category? Read(Guid id)
        {
            using var cmd = new NpgsqlCommand("SELECT * FROM categories WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@id",NpgsqlTypes.NpgsqlDbType.Uuid, id);
            

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Category
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                };
            }

            return null;
        }

        public void Update(Category entity)
        {
            throw new NotImplementedException();
        }
        public List<Category> GetAll()
        {
            var categories = new List<Category>();

            using (var cmd = new NpgsqlCommand("SELECT id, name FROM categories", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return categories;
        }
    }
}

