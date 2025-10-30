using Npgsql;
using ServiceCommon.Domain.Services;
using ServiceSales.Domain.Interfaces;
using ServiceSales.Domain.Models;

namespace ServiceSales.Infrastructure.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly IDataBase _database;

        public SaleRepository(IDataBase database)
        {
            _database = database;
        }

        public async Task<List<Sale>> GetAllAsync(CancellationToken ct = default)
        {
            var sales = new List<Sale>();
            var query = @"
                SELECT 
                    s.id, s.client_id, s.user_id, s.sale_date, s.total, 
                    s.payment_method, s.status, s.notes, s.created_at,
                    c.first_name || ' ' || c.last_name as client_name,
                    u.username as user_name
                FROM sales s
                INNER JOIN clients c ON s.client_id = c.id
                INNER JOIN users u ON s.user_id = u.id
                ORDER BY s.sale_date DESC";

            using var connection = _database.GetConnection();

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                sales.Add(MapSaleFromReader(reader));
            }

            return sales;
        }

        public async Task<List<Sale>> GetFilteredSalesAsync(SaleReportFilter filter, CancellationToken ct = default)
        {
            var sales = new List<Sale>();
            var query = @"
                SELECT 
                    s.id, s.client_id, s.user_id, s.sale_date, s.total, 
                    s.payment_method, s.status, s.notes, s.created_at,
                    c.first_name || ' ' || c.last_name as client_name,
                    u.username as user_name
                FROM sales s
                INNER JOIN clients c ON s.client_id = c.id
                INNER JOIN users u ON s.user_id = u.id
                WHERE 1=1";

            var parameters = new List<NpgsqlParameter>();

            if (filter.UserId.HasValue)
            {
                query += " AND s.user_id = @userId";
                parameters.Add(new NpgsqlParameter("@userId", filter.UserId.Value));
            }

            if (filter.ClientId.HasValue)
            {
                query += " AND s.client_id = @clientId";
                parameters.Add(new NpgsqlParameter("@clientId", filter.ClientId.Value));
            }

            if (filter.StartDate.HasValue)
            {
                query += " AND s.sale_date >= @startDate";
                parameters.Add(new NpgsqlParameter("@startDate", filter.StartDate.Value));
            }

            if (filter.EndDate.HasValue)
            {
                query += " AND s.sale_date <= @endDate";
                parameters.Add(new NpgsqlParameter("@endDate", filter.EndDate.Value));
            }

            query += " ORDER BY s.sale_date DESC";

            using var connection = _database.GetConnection();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());

            await using var reader = await command.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                sales.Add(MapSaleFromReader(reader));
            }

            return sales;
        }

        public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var query = @"
                SELECT 
                    s.id, s.client_id, s.user_id, s.sale_date, s.total, 
                    s.payment_method, s.status, s.notes, s.created_at,
                    c.first_name || ' ' || c.last_name as client_name,
                    u.username as user_name
                FROM sales s
                INNER JOIN clients c ON s.client_id = c.id
                INNER JOIN users u ON s.user_id = u.id
                WHERE s.id = @id";

            using var connection = _database.GetConnection();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync(ct);

            if (await reader.ReadAsync(ct))
            {
                return MapSaleFromReader(reader);
            }

            return null;
        }

        public async Task<List<SaleDetail>> GetSaleDetailsAsync(Guid saleId, CancellationToken ct = default)
        {
            var details = new List<SaleDetail>();
            var query = @"
                SELECT 
                    sd.id, sd.sale_id, sd.product_id, sd.quantity, 
                    sd.unit_price, sd.subtotal, sd.created_at,
                    p.title as product_name
                FROM sale_details sd
                INNER JOIN products p ON sd.product_id = p.id
                WHERE sd.sale_id = @saleId
                ORDER BY sd.created_at";

            using var connection = _database.GetConnection();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@saleId", saleId);

            await using var reader = await command.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                details.Add(new SaleDetail
                {
                    Id = reader.GetGuid(0),
                    SaleId = reader.GetGuid(1),
                    ProductId = reader.GetGuid(2),
                    Quantity = reader.GetInt32(3),
                    UnitPrice = reader.GetDecimal(4),
                    Subtotal = reader.GetDecimal(5),
                    CreatedAt = reader.GetDateTime(6),
                    ProductName = reader.IsDBNull(7) ? null : reader.GetString(7)
                });
            }

            return details;
        }

        public async Task<Dictionary<string, decimal>> GetTopProductsSalesAsync(SaleReportFilter filter, CancellationToken ct = default)
        {
            var query = @"
                SELECT 
                    p.name as product_name,
                    SUM(sd.quantity) as total_quantity
                FROM sale_details sd
                INNER JOIN sales s ON sd.sale_id = s.id
                INNER JOIN products p ON sd.product_id = p.id
                WHERE 1=1";

            var parameters = new List<NpgsqlParameter>();

            if (filter.UserId.HasValue)
            {
                query += " AND s.user_id = @userId";
                parameters.Add(new NpgsqlParameter("@userId", filter.UserId.Value));
            }

            if (filter.ClientId.HasValue)
            {
                query += " AND s.client_id = @clientId";
                parameters.Add(new NpgsqlParameter("@clientId", filter.ClientId.Value));
            }

            if (filter.StartDate.HasValue)
            {
                query += " AND s.sale_date >= @startDate";
                parameters.Add(new NpgsqlParameter("@startDate", filter.StartDate.Value));
            }

            if (filter.EndDate.HasValue)
            {
                query += " AND s.sale_date <= @endDate";
                parameters.Add(new NpgsqlParameter("@endDate", filter.EndDate.Value));
            }

            query += @"
                GROUP BY p.name
                ORDER BY total_quantity DESC";

            using var connection = _database.GetConnection();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());

            await using var reader = await command.ExecuteReaderAsync(ct);

            var result = new Dictionary<string, decimal>();
            while (await reader.ReadAsync(ct))
            {
                var productName = reader.GetString(0);
                var totalQuantity = reader.GetInt64(1);
                result[productName] = totalQuantity;
            }

            return result;
        }

        private Sale MapSaleFromReader(NpgsqlDataReader reader)
        {
            return new Sale
            {
                Id = reader.GetGuid(0),
                ClientId = reader.GetGuid(1),
                UserId = reader.GetGuid(2),
                SaleDate = reader.GetDateTime(3),
                Total = reader.GetDecimal(4),
                PaymentMethod = reader.GetString(5),
                Status = reader.GetString(6),
                Notes = reader.IsDBNull(7) ? null : reader.GetString(7),
                CreatedAt = reader.GetDateTime(8),
                ClientName = reader.IsDBNull(9) ? null : reader.GetString(9),
                UserName = reader.IsDBNull(10) ? null : reader.GetString(10)
            };
        }
    }
}
