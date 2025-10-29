using Npgsql;
using ServiceProducts.Domain.Models;
using ServiceCommon.Domain.Services;
using ServiceProducts.Domain.Interfaces;
using NpgsqlTypes;

namespace ServiceProducts.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDataBase _database;

        public ProductRepository(IDataBase database)
        {
            _database = database;
        }

        public void Create(Product product)
        {
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO products (name, description, category_id, price, stock)
                VALUES (@name, @description, @category_id, @price, @stock)", conn);

            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@description", product.Description ?? (object)DBNull.Value);

            if (product.CategoryId != Guid.Empty)
                cmd.Parameters.AddWithValue("@category_id", NpgsqlDbType.Uuid, product.CategoryId);
            else
                cmd.Parameters.AddWithValue("@category_id", DBNull.Value);

            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@stock", product.Stock);

            cmd.ExecuteNonQuery();
        }

        public Product? Read(Guid id)
        {
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand(@"
                SELECT p.*, c.name AS category_name 
                FROM products p 
                LEFT JOIN categories c ON p.category_id = c.id 
                WHERE p.id = @id", conn);

            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Uuid, id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Product
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("description")),

                    CategoryId = reader.IsDBNull(reader.GetOrdinal("category_id"))
                        ? Guid.Empty
                        : reader.GetGuid(reader.GetOrdinal("category_id")),

                    CategoryName = reader.IsDBNull(reader.GetOrdinal("category_name"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("category_name")),

                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }

            return null;
        }

        public void Update(Product product)
        {
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand(@"
                UPDATE products SET 
                    name = @name,
                    description = @description,
                    category_id = @category_id,
                    price = @price,
                    stock = @stock
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@id", product.Id);
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@description", product.Description ?? (object)DBNull.Value);

            if (product.CategoryId != Guid.Empty)
                cmd.Parameters.AddWithValue("@category_id", NpgsqlDbType.Uuid, product.CategoryId);
            else
                cmd.Parameters.AddWithValue("@category_id", DBNull.Value);

            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@stock", product.Stock);

            cmd.ExecuteNonQuery();
        }

        public void Delete(Guid id)
        {
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand("UPDATE products SET is_active = FALSE WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Uuid, id);
            cmd.ExecuteNonQuery();
        }

        public List<Product> GetAll()
        {
            var products = new List<Product>();
            using var conn = _database.GetConnection();
            using var cmd = new NpgsqlCommand(@"
                SELECT p.*, c.name as category_name 
                FROM products p 
                LEFT JOIN categories c ON p.category_id = c.id
                WHERE p.is_active = TRUE
                ORDER BY p.name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                    CategoryId = reader.IsDBNull(reader.GetOrdinal("category_id"))
                        ? Guid.Empty
                        : reader.GetGuid(reader.GetOrdinal("category_id")),
                    CategoryName = reader.IsDBNull(reader.GetOrdinal("category_name")) ? null : reader.GetString(reader.GetOrdinal("category_name")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }
            return products;
        }
    }
}
