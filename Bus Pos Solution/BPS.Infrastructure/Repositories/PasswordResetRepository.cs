using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using BPS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Infrastructure.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public PasswordResetRepository( SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int?> GetUserIdByEmailAsync(
            string email,
            CancellationToken cancellationToken)
        {
            await using var connection =
               _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_User_GetByEmail",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                256).Value = email;

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(cancellationToken);

            if (result == null ||
                result == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(result);
        }

        public async Task CreateTokenAsync(
            PasswordResetToken token,
            CancellationToken cancellationToken)
        {
            await using var connection =
               _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_PasswordResetToken_Create",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@UserId",
                SqlDbType.Int).Value =
                token.UserId;

            command.Parameters.Add(
                "@TokenHash",
                SqlDbType.Char,
                64).Value =
                token.TokenHash;

            command.Parameters.Add(
                "@ExpiresAt",
                SqlDbType.DateTime2).Value =
                token.ExpiresAt;

            await connection.OpenAsync(cancellationToken);

            await command.ExecuteNonQueryAsync(
                cancellationToken);
        }

        public async Task<int?> ResetPasswordAsync(
            string tokenHash,
            string passwordHash,
            CancellationToken cancellationToken)
        {
            await using var connection =
               _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_User_ResetPassword",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@TokenHash",
                SqlDbType.Char,
                64).Value =
                tokenHash;

            command.Parameters.Add(
                "@PasswordHash",
                SqlDbType.NVarChar,
                500).Value =
                passwordHash;

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(
                    cancellationToken);

            if (result == null ||
                result == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(result);
        }
    }
}
