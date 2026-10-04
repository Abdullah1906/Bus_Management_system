using BPS.Application.Interfaces;
using BPS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Infrastructure.Repositories
{
    public class PermissionRepository: IPermissionRepository
    {

        private readonly SqlConnectionFactory _connectionFactory;

        public PermissionRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<string>> GetPermissionsByRoleAsync(string roleName)
        {
            var permissions = new List<string>();

            await using var connection =
               _connectionFactory.CreateConnection();

            using var command =
                new SqlCommand(
                    "SP_Permission_GetByRole",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@RoleName",
                SqlDbType.NVarChar,
                50
            ).Value = roleName;

            await connection.OpenAsync();

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                permissions.Add(
                    reader["Code"]?.ToString()
                    ?? string.Empty
                );
            }

            return permissions;
        }
    }
}
