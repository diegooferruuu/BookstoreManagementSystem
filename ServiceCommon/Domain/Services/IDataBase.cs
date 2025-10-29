using Npgsql;

namespace ServiceCommon.Domain.Services
{
    public interface IDataBase
    {
        NpgsqlConnection GetConnection();
    }
}
