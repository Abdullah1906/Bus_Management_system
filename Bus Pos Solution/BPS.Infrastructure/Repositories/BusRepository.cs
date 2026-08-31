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
    public class BusRepository : IBusRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public BusRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Bus?> CreateAsync(Bus bus)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Bus_Create",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@BusName",
                SqlDbType.NVarChar,
                150)
                .Value = bus.BusName;

            command.Parameters.Add(
                "@BusNumber",
                SqlDbType.NVarChar,
                50)
                .Value = bus.BusNumber;

            command.Parameters.Add(
                "@TotalSeats",
                SqlDbType.Int)
                .Value = bus.TotalSeats;

            command.Parameters.Add(
                "@CreatedBy",
                SqlDbType.NVarChar,
                100)
                .Value = (object?)bus.CreatedBy
                    ?? DBNull.Value;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapBus(reader);
        }

        public async Task<IEnumerable<Bus>> GetAllAsync()
        {
            var buses = new List<Bus>();

            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Bus_GetAll",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                buses.Add(MapBus(reader));
            }

            return buses;
        }

        public async Task<Bus?> GetByIdAsync(int id)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Bus_GetById",
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

            return MapBus(reader);
        }

        public async Task<bool> UpdateAsync(Bus bus)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Bus_Update",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.Int)
                .Value = bus.Id;

            command.Parameters.Add(
                "@BusName",
                SqlDbType.NVarChar,
                150)
                .Value = bus.BusName;

            command.Parameters.Add(
                "@BusNumber",
                SqlDbType.NVarChar,
                50)
                .Value = bus.BusNumber;

            command.Parameters.Add(
                "@TotalSeats",
                SqlDbType.Int)
                .Value = bus.TotalSeats;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit)
                .Value = bus.IsActive;

            command.Parameters.Add(
                "@UpdatedBy",
                SqlDbType.NVarChar,
                100)
                .Value = (object?)bus.UpdatedBy
                    ?? DBNull.Value;

            await connection.OpenAsync();

            var result =
                await command.ExecuteScalarAsync();

            return result != null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Bus_Delete",
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
                    "SP_Bus_ChangeStatus",
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

            command.Parameters.Add(
                "@UpdatedBy",
                SqlDbType.NVarChar,
                100)
                .Value = DBNull.Value;

            await connection.OpenAsync();

            var result =
                await command.ExecuteScalarAsync();

            return result != null;
        }

        private static Bus MapBus(
            SqlDataReader reader)
        {
            return new Bus
            {
                Id = reader.GetInt32(
                    reader.GetOrdinal("Id")),

                BusName = reader.GetString(
                    reader.GetOrdinal("BusName")),

                BusNumber = reader.GetString(
                    reader.GetOrdinal("BusNumber")),

                TotalSeats = reader.GetInt32(
                    reader.GetOrdinal("TotalSeats")),

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
