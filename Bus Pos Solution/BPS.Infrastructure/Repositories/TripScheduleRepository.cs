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
    public class TripScheduleRepository
    : ITripScheduleRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public TripScheduleRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Trip?> CreateScheduleAsync(
            Trip trip)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_CreateTripSchedule",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@BusId",
                SqlDbType.Int)
                .Value = trip.BusId;

            command.Parameters.Add(
                "@RouteId",
                SqlDbType.Int)
                .Value = trip.RouteId;

            command.Parameters.Add(
                "@TripDate",
                SqlDbType.Date)
                .Value = trip.TripDate.Date;

            command.Parameters.Add(
                "@DepartureTime",
                SqlDbType.Time)
                .Value = trip.DepartureTime;

            command.Parameters.Add(
                "@ArrivalTime",
                SqlDbType.Time)
                .Value =
                    trip.ArrivalTime.HasValue
                        ? trip.ArrivalTime.Value
                        : DBNull.Value;

            var fareParameter =
                command.Parameters.Add(
                    "@Fare",
                    SqlDbType.Decimal);

            fareParameter.Precision = 18;
            fareParameter.Scale = 2;
            fareParameter.Value = trip.Fare;

            command.Parameters.Add(
                "@CreatedBy",
                SqlDbType.NVarChar,
                100)
                .Value =
                    (object?)trip.CreatedBy
                    ?? DBNull.Value;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Trip
            {
                Id = reader.GetInt64(
                    reader.GetOrdinal("Id")),

                BusId = reader.GetInt32(
                    reader.GetOrdinal("BusId")),

                RouteId = reader.GetInt32(
                    reader.GetOrdinal("RouteId")),

                TripDate = reader.GetDateTime(
                    reader.GetOrdinal("TripDate")),

                DepartureTime = reader.GetTimeSpan(
                    reader.GetOrdinal("DepartureTime")),

                ArrivalTime =
                    reader.IsDBNull(
                        reader.GetOrdinal("ArrivalTime"))
                        ? null
                        : reader.GetTimeSpan(
                            reader.GetOrdinal("ArrivalTime")),

                Fare = reader.GetDecimal(
                    reader.GetOrdinal("Fare")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt")),

                CreatedBy =
                    reader.IsDBNull(
                        reader.GetOrdinal("CreatedBy"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("CreatedBy"))
            };
        }
    }
}
