using BPS.Application.Interfaces;
using BPS.Domain.Entities;
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
    public class RouteRepository : IRouteRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public RouteRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Route?> CreateAsync(Route route)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Route_Create",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@FromPlace",
                SqlDbType.NVarChar,
                150)
                .Value = route.FromPlace;

            command.Parameters.Add(
                "@ToPlace",
                SqlDbType.NVarChar,
                150)
                .Value = route.ToPlace;

            var distanceParameter =
                command.Parameters.Add(
                    "@DistanceKm",
                    SqlDbType.Decimal);

            distanceParameter.Precision = 10;
            distanceParameter.Scale = 2;

            distanceParameter.Value =
                route.DistanceKm.HasValue
                    ? route.DistanceKm.Value
                    : DBNull.Value;

            command.Parameters.Add(
                "@EstimatedMinutes",
                SqlDbType.Int)
                .Value =
                    route.EstimatedMinutes.HasValue
                        ? route.EstimatedMinutes.Value
                        : DBNull.Value;

            command.Parameters.Add(
                "@CreatedBy",
                SqlDbType.NVarChar,
                100)
                .Value =
                    (object?)route.CreatedBy
                    ?? DBNull.Value;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapRoute(reader);
        }

        public async Task<IEnumerable<Route>> GetAllAsync()
        {
            var routes = new List<Route>();

            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Route_GetAll",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                routes.Add(MapRoute(reader));
            }

            return routes;
        }

        public async Task<Route?> GetByIdAsync(int id)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Route_GetById",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.Int)
                .Value = id;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapRoute(reader);
        }

        public async Task<bool> UpdateAsync(Route route)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Route_Update",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.Int)
                .Value = route.Id;

            command.Parameters.Add(
                "@FromPlace",
                SqlDbType.NVarChar,
                150)
                .Value = route.FromPlace;

            command.Parameters.Add(
                "@ToPlace",
                SqlDbType.NVarChar,
                150)
                .Value = route.ToPlace;

            var distanceParameter =
                command.Parameters.Add(
                    "@DistanceKm",
                    SqlDbType.Decimal);

            distanceParameter.Precision = 10;
            distanceParameter.Scale = 2;

            distanceParameter.Value =
                route.DistanceKm.HasValue
                    ? route.DistanceKm.Value
                    : DBNull.Value;

            command.Parameters.Add(
                "@EstimatedMinutes",
                SqlDbType.Int)
                .Value =
                    route.EstimatedMinutes.HasValue
                        ? route.EstimatedMinutes.Value
                        : DBNull.Value;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit)
                .Value = route.IsActive;

            command.Parameters.Add(
                "@UpdatedBy",
                SqlDbType.NVarChar,
                100)
                .Value =
                    (object?)route.UpdatedBy
                    ?? DBNull.Value;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            return await reader.ReadAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Route_Delete",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.Int)
                .Value = id;

            await connection.OpenAsync();

            var result =
                await command.ExecuteScalarAsync();

            return result != null;
        }

        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Route_ChangeStatus",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.Int)
                .Value = id;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit)
                .Value = isActive;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            return await reader.ReadAsync();
        }

        private static Route MapRoute(
            SqlDataReader reader)
        {
            return new Route
            {
                Id = reader.GetInt32(
                    reader.GetOrdinal("Id")),

                FromPlace = reader.GetString(
                    reader.GetOrdinal("FromPlace")),

                ToPlace = reader.GetString(
                    reader.GetOrdinal("ToPlace")),

                DistanceKm = reader.IsDBNull(
                    reader.GetOrdinal("DistanceKm"))
                    ? null
                    : reader.GetDecimal(
                        reader.GetOrdinal("DistanceKm")),

                EstimatedMinutes = reader.IsDBNull(
                    reader.GetOrdinal("EstimatedMinutes"))
                    ? null
                    : reader.GetInt32(
                        reader.GetOrdinal("EstimatedMinutes")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt")),

                CreatedBy = reader.IsDBNull(
                    reader.GetOrdinal("CreatedBy"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("CreatedBy")),

                UpdatedAt = reader.IsDBNull(
                    reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("UpdatedAt")),

                UpdatedBy = reader.IsDBNull(
                    reader.GetOrdinal("UpdatedBy"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}
