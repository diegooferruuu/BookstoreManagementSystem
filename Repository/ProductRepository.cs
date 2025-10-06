using Npgsql;
using BookstoreManagementSystem.Services;
using BookstoreManagementSystem.Models;

namespace BookstoreManagementSystem.Repository
{
    public class ProductRepository : IDataBase<Product>
    {
        private readonly NpgsqlConnection _connection;

        public ProductRepository()
        {
            _connection = DataBaseConnection.Instance.GetConnection();
        }

        public void Create(Product product)
        {
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO products (name, description, category, price, stock, sku)
                VALUES (@name, @description, @category, @price, @stock)", _connection);

            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@description", product.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@category", product.Category ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@stock", product.Stock);

            cmd.ExecuteNonQuery();

        }

        public Product Read(int id)
        {
            using var cmd = new NpgsqlCommand("SELECT * FROM products WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Product
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                    Category = reader.IsDBNull(reader.GetOrdinal("category")) ? null : reader.GetString(reader.GetOrdinal("category")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }

            return null;

        }

        public void Update(Product product)
        {
            using var cmd = new NpgsqlCommand(@"
                UPDATE products SET 
                    name = @name,
                    description = @description,
                    category = @category,
                    price = @price,
                    stock = @stock,
                    sku = @sku
                WHERE id = @id", _connection);

            cmd.Parameters.AddWithValue("@id", product.Id);
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@description", product.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@category", product.Category ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@stock", product.Stock);

            cmd.ExecuteNonQuery();

        }

        public void Delete(int id)
        {
            using var cmd = new NpgsqlCommand("DELETE FROM products WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Product> GetAll()
        {
            var products = new List<Product>();
            using var cmd = new NpgsqlCommand("SELECT * FROM products", _connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                    Category = reader.IsDBNull(reader.GetOrdinal("category")) ? null : reader.GetString(reader.GetOrdinal("category")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }
            return products;

        }
    }
}
