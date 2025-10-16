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
        private NpgsqlConnection _sharedConnection;

        private DataBaseConnection()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            _connectionString = config.GetConnectionString("DefaultConnection");
            _sharedConnection = new NpgsqlConnection(_connectionString);
            _sharedConnection.Open();
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
            if (_sharedConnection == null)
            {
                _sharedConnection = new NpgsqlConnection(_connectionString);
            }

            if (_sharedConnection.State != System.Data.ConnectionState.Open)
            {
                try { _sharedConnection.Open(); }
                catch
                {
                    // recreate connection if broken
                    try { _sharedConnection.Dispose(); } catch { }
                    _sharedConnection = new NpgsqlConnection(_connectionString);
                    _sharedConnection.Open();
                }
            }

            return _sharedConnection;
        }




    }
}
