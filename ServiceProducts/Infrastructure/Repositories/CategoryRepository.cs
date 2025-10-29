using Npgsql;
using ServiceProducts.Domain.Models;
using ServiceCommon.Domain.Services;
using ServiceProducts.Domain.Interfaces;

namespace ServiceProducts.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDataBase _database;

        public CategoryRepository(IDataBase database)
        {
            _database = database;
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
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand("SELECT * FROM categories WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);

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
            using var conn = _database.GetConnection();
            using (var cmd = new NpgsqlCommand("SELECT id, name FROM categories", conn))
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
            return categories;
        }
    }
}
