using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using BPS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System.Data;
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

        public async Task<User?> GetByUsernameAsync(string username)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_User_GetByUsername",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Username",
                SqlDbType.NVarChar, 100)
                .Value = username;

            await connection.OpenAsync();

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

                Email = reader["Email"]?.ToString() ?? "",
                PhoneNumber = reader["PhoneNumber"]?.ToString() ?? "",

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

        public async Task<int> CreateAsync(User user)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_User_Create",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Username",
                SqlDbType.NVarChar, 100)
                .Value = user.Username;

            command.Parameters.Add(
                "@PasswordHash",
                SqlDbType.NVarChar, 500)
                .Value = user.PasswordHash;

            command.Parameters.Add(
                "@FullName",
                SqlDbType.NVarChar, 150)
                .Value = user.FullName;

            command.Parameters.Add(
                "@Role",
                SqlDbType.NVarChar, 50)
                .Value = user.Role;

            command.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar, 255)
                .Value = user.Email;

            command.Parameters.Add(
                "@PhoneNumber",
                SqlDbType.NVarChar, 20)
                .Value = user.PhoneNumber;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit)
                .Value = user.IsActive;

            command.Parameters.Add(
                "@CreatedBy",
                SqlDbType.NVarChar, 100)
                .Value = user.CreatedBy;

            command.Parameters.Add(
                "@UpdatedBy",
                SqlDbType.NVarChar, 100)
                .Value = user.UpdatedBy;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }

    }

}
