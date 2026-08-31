using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using BPS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BPS.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public BookingRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<TripSeat>> LockSeatsAsync(
            long tripId,
            IEnumerable<long> tripSeatIds,
            long customerId)
        {
            var ids = tripSeatIds
                .Distinct()
                .ToList();

            if (ids.Count == 0)
                throw new ArgumentException(
                    "At least one seat is required.");

            var json =
                JsonSerializer.Serialize(ids);

            var seats = new List<TripSeat>();

            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_LockSeats",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@TripId",
                SqlDbType.BigInt)
                .Value = tripId;

            command.Parameters.Add(
                "@TripSeatIds",
                SqlDbType.NVarChar)
                .Value = json;

            command.Parameters.Add(
                "@CustomerId",
                SqlDbType.BigInt)
                .Value = customerId;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                seats.Add(new TripSeat
                {
                    Id = reader.GetInt64(
                        reader.GetOrdinal("TripSeatId")),

                    TripId = reader.GetInt64(
                        reader.GetOrdinal("TripId")),

                    BusSeatId = reader.GetInt64(
                        reader.GetOrdinal("BusSeatId")),

                    SeatNumber = reader.GetString(
                        reader.GetOrdinal("SeatNumber")),

                    Status = reader.GetByte(
                        reader.GetOrdinal("Status")),

                    LockedUntil = reader.GetDateTime(
                        reader.GetOrdinal("LockedUntil"))
                });
            }

            return seats;
        }


        public async Task<Booking?> ConfirmBookingAsync(
        long tripId,
        long customerId,
        string paymentMethod,
        string? transactionId,
        string passengersJson)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_ConfirmBooking",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@TripId",
                SqlDbType.BigInt)
                .Value = tripId;

            command.Parameters.Add(
                "@CustomerId",
                SqlDbType.BigInt)
                .Value = customerId;

            command.Parameters.Add(
                "@PassengersJson",
                SqlDbType.NVarChar)
                .Value = passengersJson;

            command.Parameters.Add(
                "@PaymentMethod",
                SqlDbType.NVarChar,
                50)
                .Value = paymentMethod;

            command.Parameters.Add(
                "@TransactionId",
                SqlDbType.NVarChar,
                100)
                .Value =
                    (object?)transactionId
                    ?? DBNull.Value;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Booking
            {
                Id = reader.GetInt64(
                    reader.GetOrdinal("BookingId")),

                PNR = reader.GetString(
                    reader.GetOrdinal("PNR")),

                TripId = reader.GetInt64(
                    reader.GetOrdinal("TripId")),

                CustomerId = reader.GetInt64(
                    reader.GetOrdinal("CustomerId")),

                TotalAmount = reader.GetDecimal(
                    reader.GetOrdinal("TotalAmount")),

                BookingStatus = reader.GetByte(
                    reader.GetOrdinal("BookingStatus")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt")),

                ConfirmedAt =
                    reader.IsDBNull(
                        reader.GetOrdinal("ConfirmedAt"))
                        ? null
                        : reader.GetDateTime(
                            reader.GetOrdinal("ConfirmedAt"))
            };
        }

        public async Task<int> ReleaseExpiredSeatLocksAsync()
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command =
                new SqlCommand(
                    "SP_ReleaseExpiredSeatLocks",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            await connection.OpenAsync();

            var result =
                await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }
    }
}
