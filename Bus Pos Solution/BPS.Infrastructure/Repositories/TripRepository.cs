using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using BPS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System.Data;


namespace BPS.Infrastructure.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public TripRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<TripRecord?> CreateAsync(
            TripRecord trip)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Trip_Create",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@PlaceId",
                SqlDbType.Int)
                .Value = trip.PlaceId;

            command.Parameters.Add(
                "@TripDate",
                SqlDbType.Date)
                .Value = trip.TripDate.Date;

            command.Parameters.Add(
                "@TipStatus",
                SqlDbType.Bit)
                .Value = trip.TipStatus;

            var tipParameter =
                command.Parameters.Add(
                    "@TipAmount",
                    SqlDbType.Decimal);
        

            tipParameter.Precision = 18;
            tipParameter.Scale = 2;
            tipParameter.Value = trip.TipAmount;

            command.Parameters.Add(
                "@CreatedBy",
                SqlDbType.NVarChar,
                100)
                .Value = (object?)trip.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapTrip(reader);
        }

        public async Task<TripRecord?> UpdateAsync(TripRecord trip)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Trip_Update",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt)
                .Value = trip.Id;

            command.Parameters.Add(
                "@PlaceId",
                SqlDbType.Int)
                .Value = trip.PlaceId;

            command.Parameters.Add(
                "@TripDate",
                SqlDbType.Date)
                .Value = trip.TripDate.Date;

            command.Parameters.Add(
                "@TipStatus",
                SqlDbType.Bit)
                .Value = trip.TipStatus;

            var tipParameter =
                command.Parameters.Add(
                    "@TipAmount",
                    SqlDbType.Decimal);

            tipParameter.Precision = 18;
            tipParameter.Scale = 2;
            tipParameter.Value = trip.TipAmount;

            command.Parameters.Add(
                "@UpdatedBy",
                SqlDbType.NVarChar,
                100)
                .Value = (object?)trip.UpdatedBy
                    ?? DBNull.Value;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapTrip(reader);
        }

        public async Task<TripRecord?> GetByIdAsync(
            long id)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Trip_GetById",
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

            return MapTrip(reader);
        }


        public async Task<IEnumerable<TripRecord>> GetAllAsync()
        {
            var trips =
                new List<TripRecord>();

            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Trip_GetAll",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                trips.Add(MapTrip(reader));
            }

            return trips;
        }

        public async Task<bool> DeleteAsync(long id, string? updatedBy)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_Trip_Delete",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt)
                .Value = id;

            command.Parameters.Add(
                "@UpdatedBy",
                SqlDbType.NVarChar,
                100)
                .Value = (object?)updatedBy
                    ?? DBNull.Value;

            await connection.OpenAsync();

            var result =
                Convert.ToInt32(
                    await command.ExecuteScalarAsync());

            return result > 0;
        }

        private static TripRecord MapTrip(
            SqlDataReader reader)
        {
            return new TripRecord
            {
                Id = reader.GetInt64(
                    reader.GetOrdinal("Id")),

                PlaceId = reader.GetInt32(
                    reader.GetOrdinal("PlaceId")),

                TripDate = reader.GetDateTime(
                    reader.GetOrdinal("TripDate")),

                TipStatus = reader.GetBoolean(
                    reader.GetOrdinal("TipStatus")),

                TipAmount = reader.GetDecimal(
                    reader.GetOrdinal("TipAmount")),

                Price = reader.GetDecimal(
                    reader.GetOrdinal("Price")),

                Total = reader.GetDecimal(
                    reader.GetOrdinal("Total")),


            
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
                        reader.GetOrdinal("UpdatedBy")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive"))
            };
        }
}   }
