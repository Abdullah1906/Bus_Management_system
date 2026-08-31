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
    public class BusSeatRepository
    : IBusSeatRepository
    {
        private readonly SqlConnectionFactory
            _connectionFactory;

        public BusSeatRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory =
                connectionFactory;
        }

        public async Task<BusSeat?> CreateAsync(
            BusSeat seat)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_BusSeat_Create",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@BusId",
                SqlDbType.Int)
                .Value = seat.BusId;

            command.Parameters.Add(
                "@SeatNumber",
                SqlDbType.NVarChar,
                20)
                .Value = seat.SeatNumber;

            command.Parameters.Add(
                "@RowNumber",
                SqlDbType.Int)
                .Value = seat.RowNumber;

            command.Parameters.Add(
                "@ColumnNumber",
                SqlDbType.Int)
                .Value = seat.ColumnNumber;

            command.Parameters.Add(
                "@IsWindow",
                SqlDbType.Bit)
                .Value = seat.IsWindow;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapSeat(reader);
        }

        public async Task<IEnumerable<BusSeat>>
            GetByBusIdAsync(int busId)
        {
            var seats =
                new List<BusSeat>();

            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_BusSeat_GetByBusId",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@BusId",
                SqlDbType.Int)
                .Value = busId;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                seats.Add(MapSeat(reader));
            }

            return seats;
        }

        public async Task<BusSeat?> GetByIdAsync(
            long id)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_BusSeat_GetById",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt)
                .Value = id;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapSeat(reader);
        }

        public async Task<bool> UpdateAsync(
            BusSeat seat)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_BusSeat_Update",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt)
                .Value = seat.Id;

            command.Parameters.Add(
                "@SeatNumber",
                SqlDbType.NVarChar,
                20)
                .Value = seat.SeatNumber;

            command.Parameters.Add(
                "@RowNumber",
                SqlDbType.Int)
                .Value = seat.RowNumber;

            command.Parameters.Add(
                "@ColumnNumber",
                SqlDbType.Int)
                .Value = seat.ColumnNumber;

            command.Parameters.Add(
                "@IsWindow",
                SqlDbType.Bit)
                .Value = seat.IsWindow;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit)
                .Value = seat.IsActive;

            await connection.OpenAsync();

            var result =
                await command.ExecuteScalarAsync();

            return result != null;
        }

        public async Task<bool> DeleteAsync(
            long id)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_BusSeat_Delete",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt)
                .Value = id;

            await connection.OpenAsync();

            var result =
                await command.ExecuteScalarAsync();

            return result != null;
        }

        public async Task<bool> ChangeStatusAsync(
            long id,
            bool isActive)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_BusSeat_ChangeStatus",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt)
                .Value = id;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit)
                .Value = isActive;

            await connection.OpenAsync();

            var result =
                await command.ExecuteScalarAsync();

            return result != null;
        }

        private static BusSeat MapSeat(
            SqlDataReader reader)
        {
            return new BusSeat
            {
                Id = reader.GetInt64(
                    reader.GetOrdinal("Id")),

                BusId = reader.GetInt32(
                    reader.GetOrdinal("BusId")),

                SeatNumber = reader.GetString(
                    reader.GetOrdinal("SeatNumber")),

                RowNumber = reader.GetInt32(
                    reader.GetOrdinal("RowNumber")),

                ColumnNumber = reader.GetInt32(
                    reader.GetOrdinal("ColumnNumber")),

                IsWindow = reader.GetBoolean(
                    reader.GetOrdinal("IsWindow")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt"))
            };
        }
    }
}
