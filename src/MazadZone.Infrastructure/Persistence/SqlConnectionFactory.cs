using System.Data;
using Microsoft.Data.SqlClient;
using MazadZone.Application.Common.Interfaces;

namespace MazadZone.Infrastructure.Persistence;

public sealed class SqlConnectionFactory(string _connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        // We do not open it here. We let Dapper or the caller open it when needed
        // to keep the connection lifecycle as short as possible.
        return connection; 
    }
}