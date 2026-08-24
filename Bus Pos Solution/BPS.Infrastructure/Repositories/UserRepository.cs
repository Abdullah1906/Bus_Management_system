using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using BPS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
namespace BPS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public UserRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByUsernameAsync(
            string username)
        {
            const string sql = """
            SELECT
                Id,
                Username,
                PasswordHash,
                FullName,
                Role,
                IsActive,
                CreatedAt,
                UpdatedAt
            FROM Users
            WHERE Username = @Username
            """;

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@Username",
                username);

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new User
            {
                Id = reader.GetInt32(
                    reader.GetOrdinal("Id")),

                Username = reader.GetString(
                    reader.GetOrdinal("Username")),

                PasswordHash = reader.GetString(
                    reader.GetOrdinal("PasswordHash")),

                FullName = reader.GetString(
                    reader.GetOrdinal("FullName")),

                Role = reader.GetString(
                    reader.GetOrdinal("Role")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt")),

                UpdatedAt = reader.IsDBNull(
                    reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
