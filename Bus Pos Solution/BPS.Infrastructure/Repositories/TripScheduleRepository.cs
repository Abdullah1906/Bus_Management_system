using BPS.Application.DTOs.TripSchedules;
using BPS.Application.DTOs.TripSearch;
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


        public async Task<IReadOnlyList<TripScheduleDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
        {
            var result = new List<TripScheduleDto>();

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync(cancellationToken);


            await using var command =
                new SqlCommand(
                    "dbo.SP_TripSchedule_GetAll",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;


            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);


            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(
                    new TripScheduleDto
                    {
                        Id =
                            reader.GetInt64(
                                reader.GetOrdinal("Id")),

                        BusId =
                            reader.GetInt32(
                                reader.GetOrdinal("BusId")),

                        BusName =
                            reader.GetString(
                                reader.GetOrdinal("BusName")),

                        BusNumber =
                            reader.GetString(
                                reader.GetOrdinal("BusNumber")),

                        RouteId =
                            reader.GetInt32(
                                reader.GetOrdinal("RouteId")),

                        FromPlace =
                            reader.GetString(
                                reader.GetOrdinal("FromPlace")),

                        ToPlace =
                            reader.GetString(
                                reader.GetOrdinal("ToPlace")),

                        TripDate =
                            reader.GetDateTime(
                                reader.GetOrdinal("TripDate")),

                        DepartureTime =
                            reader.GetTimeSpan(
                                reader.GetOrdinal("DepartureTime")),

                        ArrivalTime =
                            reader.IsDBNull(
                                reader.GetOrdinal("ArrivalTime"))
                                ? null
                                : reader.GetTimeSpan(
                                    reader.GetOrdinal("ArrivalTime")),

                        Fare =
                            reader.GetDecimal(
                                reader.GetOrdinal("Fare")),

                        IsActive =
                            reader.GetBoolean(
                                reader.GetOrdinal("IsActive")),

                        CreatedAt =
                            reader.GetDateTime(
                                reader.GetOrdinal("CreatedAt")),

                        CreatedBy =
                            reader.IsDBNull(
                                reader.GetOrdinal("CreatedBy"))
                                ? null
                                : reader.GetString(
                                    reader.GetOrdinal("CreatedBy"))
                    });
            }


            return result;
        }

        public async Task<TripScheduleDto?> GetByIdAsync(long id,CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync(cancellationToken);

            await using var command =
                new SqlCommand(
                    "dbo.SP_TripSchedule_GetById",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt).Value = id;

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            if (!await reader.ReadAsync(
                cancellationToken))
            {
                return null;
            }

            return new TripScheduleDto
            {
                Id = reader.GetInt64(
                    reader.GetOrdinal("Id")),

                BusId = reader.GetInt32(
                    reader.GetOrdinal("BusId")),

                BusName = reader.GetString(
                    reader.GetOrdinal("BusName")),

                BusNumber = reader.GetString(
                    reader.GetOrdinal("BusNumber")),

                RouteId = reader.GetInt32(
                    reader.GetOrdinal("RouteId")),

                FromPlace = reader.GetString(
                    reader.GetOrdinal("FromPlace")),

                ToPlace = reader.GetString(
                    reader.GetOrdinal("ToPlace")),

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
        public async Task<bool> UpdateAsync(long id,Trip trip, CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync(cancellationToken);

            await using var command =
                new SqlCommand(
                    "dbo.SP_TripSchedule_Update",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt).Value = id;

            command.Parameters.Add(
                "@BusId",
                SqlDbType.Int).Value = trip.BusId;

            command.Parameters.Add(
                "@RouteId",
                SqlDbType.Int).Value = trip.RouteId;

            command.Parameters.Add(
                "@TripDate",
                SqlDbType.Date).Value =
                    trip.TripDate.Date;

            command.Parameters.Add(
                "@DepartureTime",
                SqlDbType.Time).Value =
                    trip.DepartureTime;

            command.Parameters.Add(
                "@ArrivalTime",
                SqlDbType.Time).Value =
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
                "@IsActive",
                SqlDbType.Bit).Value =
                    trip.IsActive;

            var result =
                await command.ExecuteScalarAsync(
                    cancellationToken);

            return result != null &&
                   Convert.ToBoolean(result);
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync(cancellationToken);

            await using var command =
                new SqlCommand(
                    "dbo.SP_TripSchedule_Delete",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt).Value = id;

            var result =
                await command.ExecuteScalarAsync(
                    cancellationToken);

            return result != null &&
                   Convert.ToBoolean(result);
        }
        public async Task<bool> ChangeStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync(cancellationToken);

            await using var command =
                new SqlCommand(
                    "dbo.SP_TripSchedule_ChangeStatus",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@Id",
                SqlDbType.BigInt).Value = id;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value = isActive;

            var result =
                await command.ExecuteScalarAsync(
                    cancellationToken);

            return result != null &&
                   Convert.ToBoolean(result);
        }

        public async Task<IEnumerable<TripSearchResponseDto>> SearchAsync(
        string fromPlace,
        string toPlace,
        DateTime tripDate)
        {
            var trips = new List<TripSearchResponseDto>();

            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "SP_Trip_Search",
                connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add(
                "@FromPlace",
                SqlDbType.NVarChar, 150
            ).Value = fromPlace;

            command.Parameters.Add(
                "@ToPlace",
                SqlDbType.NVarChar, 150
            ).Value = toPlace;

            command.Parameters.Add(
                "@TripDate",
                SqlDbType.Date
            ).Value = tripDate.Date;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                trips.Add(new TripSearchResponseDto
                {
                    TripId = reader.GetInt64(
                        reader.GetOrdinal("TripId")),

                    BusId = reader.GetInt32(
                        reader.GetOrdinal("BusId")),

                    BusName = reader.GetString(
                        reader.GetOrdinal("BusName")),

                    BusNumber = reader.GetString(
                        reader.GetOrdinal("BusNumber")),

                    RouteId = reader.GetInt32(
                        reader.GetOrdinal("RouteId")),

                    FromPlace = reader.GetString(
                        reader.GetOrdinal("FromPlace")),

                    ToPlace = reader.GetString(
                        reader.GetOrdinal("ToPlace")),

                    TripDate = reader.GetDateTime(
                        reader.GetOrdinal("TripDate")),

                    DepartureTime = reader.GetTimeSpan(
                        reader.GetOrdinal("DepartureTime")),

                    ArrivalTime = reader.IsDBNull(
                        reader.GetOrdinal("ArrivalTime"))
                        ? null
                        : reader.GetTimeSpan(
                            reader.GetOrdinal("ArrivalTime")),

                    Fare = reader.GetDecimal(
                        reader.GetOrdinal("Fare")),

                    TotalSeats = reader.GetInt32(
                        reader.GetOrdinal("TotalSeats")),

                    AvailableSeats = reader.GetInt32(
                        reader.GetOrdinal("AvailableSeats"))
                });
            }

            return trips;
        }
    }
}
