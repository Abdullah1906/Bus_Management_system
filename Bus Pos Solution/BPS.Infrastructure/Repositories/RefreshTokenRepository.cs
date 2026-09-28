using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.Data.SqlClient;
using BPS.Infrastructure.Data;
using System.Data;


namespace BPS.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public RefreshTokenRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("sp_RefreshToken_GetByToken", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(
                "@Token",
                SqlDbType.NVarChar,
                500)
                .Value = token;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RefreshToken
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Token = reader.GetString(reader.GetOrdinal("Token")),
                    ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
                    IsRevoked = reader.GetBoolean(reader.GetOrdinal("IsRevoked")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }
            return null;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("sp_RefreshToken_Insert", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(
                "@UserId",
                SqlDbType.Int)
                .Value = refreshToken.UserId;

            command.Parameters.Add(
                "@Token",
                SqlDbType.NVarChar,
                500)
                .Value = refreshToken.Token;

            command.Parameters.Add(
                "@ExpiresAt",
                SqlDbType.DateTime2)
                .Value = refreshToken.ExpiresAt;

            command.Parameters.Add(
                "@IsRevoked",
                SqlDbType.Bit)
                .Value = refreshToken.IsRevoked;

            await connection.OpenAsync();
            var newId = await command.ExecuteScalarAsync();
            if (newId != null && int.TryParse(newId.ToString(), out var id))
            {
                refreshToken.Id = id;
            }
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "sp_RefreshToken_Update",
                    connection);
            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.Int)
                .Value = refreshToken.Id;

            command.Parameters.Add(
                "@IsRevoked",
                SqlDbType.Bit)
                .Value = refreshToken.IsRevoked;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task RevokeAllForUserAsync(int userId)
        {
            await using var connection =
                 _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "sp_RefreshToken_RevokeAllForUser",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@UserId",
                SqlDbType.Int)
                .Value = userId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
