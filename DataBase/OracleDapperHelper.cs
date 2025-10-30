using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using Microsoft.Extensions.Logging;

namespace library_management_system.DataBase
{
    public class OracleDapperHelper : IDisposable
    {
        private readonly string _connectionString;
        private readonly ILogger<OracleDapperHelper> _logger;
        private OracleConnection _connection;

        public OracleDapperHelper(string connectionString, ILogger<OracleDapperHelper> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connection = new OracleConnection(_connectionString);
        }

        public OracleConnection GetConnection()
        {
            var conn = new OracleConnection(_connectionString);
            conn.Open();
            return conn;
        }

        private void EnsureOpen()
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                    _logger.LogDebug("Database connection opened successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open database connection");
                throw;
            }
        }

        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            try
            {
                EnsureOpen();
                _logger.LogDebug($"Executing query: {sql}");
                return _connection.Query<T>(sql, param);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing query: {sql}");
                throw;
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            try
            {
                EnsureOpen();
                _logger.LogDebug($"Executing async query: {sql}");
                return await _connection.QueryAsync<T>(sql, param);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing async query: {sql}");
                throw;
            }
        }

        public T QuerySingle<T>(string sql, object param = null)
        {
            try
            {
                EnsureOpen();
                _logger.LogDebug($"Executing single query: {sql}");
                return _connection.QuerySingleOrDefault<T>(sql, param);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing single query: {sql}");
                throw;
            }
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object param = null)
        {
            try
            {
                EnsureOpen();
                _logger.LogDebug($"Executing async single query: {sql}");
                return await _connection.QuerySingleOrDefaultAsync<T>(sql, param);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing async single query: {sql}");
                throw;
            }
        }

        public int Execute(string sql, object param = null)
        {
            try
            {
                EnsureOpen();
                _logger.LogDebug($"Executing command: {sql}");
                return _connection.Execute(sql, param);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing command: {sql}");
                throw;
            }
        }

        public async Task<int> ExecuteAsync(string sql, object param = null)
        {
            try
            {
                EnsureOpen();
                _logger.LogDebug($"Executing async command: {sql}");
                return await _connection.ExecuteAsync(sql, param);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing async command: {sql}");
                throw;
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {
                EnsureOpen();
                _logger.LogDebug($"Executing scalar async: {sql}");
                return await _connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing scalar async: {sql}");
                throw;
            }
        }

        public async Task<int?> ExecuteScalarAsyncNullable(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            EnsureOpen();
            var result = await _connection.ExecuteScalarAsync<object>(
                sql,
                param,
                transaction,
                commandTimeout,
                commandType);
            return result as int?;
        }

        public void Dispose()
        {
            try
            {
                if (_connection != null)
                {
                    if (_connection.State != ConnectionState.Closed)
                    {
                        _connection.Close();
                        _logger.LogDebug("Database connection closed");
                    }
                    _connection.Dispose();
                    _connection = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing database connection");
            }
        }
    }
}