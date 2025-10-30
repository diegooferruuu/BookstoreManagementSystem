using Npgsql;
using NpgsqlTypes;
using ServiceProducts.Domain.Models;
using ServiceProducts.Domain.Reports;
using ServiceProducts.Domain.Interfaces;
using ServiceCommon.Domain.Services;

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
                SELECT p.*, c.name AS category_name 
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

        // ---------------------------------------------
        // MÉTODO NUEVO: CONSULTA PARA REPORTES
        // ---------------------------------------------
        public async Task<IReadOnlyList<ProductReportRow>> GetForReportAsync(
            decimal? priceMin, decimal? priceMax, Guid? categoryId, CancellationToken ct)
        {
            var rows = new List<ProductReportRow>();
            using var conn = _database.GetConnection();

            // Si no se pasa rango, obtener el máximo actual
            if (priceMin == null) priceMin = 0;

            if (priceMax == null)
            {
                using var maxCmd = new NpgsqlCommand("SELECT COALESCE(MAX(price), 0) FROM products WHERE is_active = TRUE;", conn);
                priceMax = Convert.ToDecimal(maxCmd.ExecuteScalar());
            }

            var sql = @"
                SELECT p.name, 
                       c.name AS category, 
                       COALESCE(p.description, '') AS description,
                       p.price, 
                       p.stock
                FROM products p
                JOIN categories c ON c.id = p.category_id
                WHERE p.is_active = TRUE
                  AND p.price >= @min
                  AND p.price <= @max
                  AND (@cat IS NULL OR p.category_id = @cat)
                ORDER BY c.name, p.name;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@min", NpgsqlDbType.Numeric).Value = priceMin.Value;
            cmd.Parameters.Add("@max", NpgsqlDbType.Numeric).Value = priceMax.Value;

            if (categoryId == null || categoryId == Guid.Empty)
                cmd.Parameters.Add("@cat", NpgsqlDbType.Uuid).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@cat", NpgsqlDbType.Uuid).Value = categoryId.Value;

            using var reader = await cmd.ExecuteReaderAsync(ct);
            int i = 0;

            while (await reader.ReadAsync(ct))
            {
                rows.Add(new ProductReportRow
                {
                    Nro = ++i,
                    Name = reader.GetString(0),
                    Category = reader.GetString(1),
                    Description = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Stock = reader.GetInt32(4)
                });
            }

            return rows;
        }
    }
}
