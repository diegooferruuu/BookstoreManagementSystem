using Npgsql;
using Microsoft.Extensions.Configuration;
using ServiceCommon.Domain.Services;

namespace ServiceCommon.Infrastructure.DataBase
{
    public class DataBaseConnection : IDataBase
    {
        private static DataBaseConnection? _instance;
        private static readonly object _padlock = new object();
        private readonly string _connectionString;

        private DataBaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static DataBaseConnection GetInstance(string connectionString)
        {
            if (_instance == null)
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new DataBaseConnection(connectionString);
                    }
                }
            }
            return _instance;
        }

        public NpgsqlConnection GetConnection()
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
