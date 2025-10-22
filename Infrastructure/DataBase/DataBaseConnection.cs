using Npgsql;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace BookstoreManagementSystem.Infrastructure.DataBase
{
    public class DataBaseConnection
    {
        private static DataBaseConnection instance;
        private static readonly object padlock = new object();
        private readonly string _connectionString;

        private DataBaseConnection()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public static DataBaseConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new DataBaseConnection();
                        }

                    }
                }
                return instance;
            }

        }

        public NpgsqlConnection GetConnection()
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
