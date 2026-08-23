using System.Data;
using Npgsql;

namespace FacturaBit.Infrastructure.Persistence;

public class DapperDBContext
{
    private readonly string _connectionString;

    public DapperDBContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}