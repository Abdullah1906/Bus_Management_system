using BPS.Application.DTOs.Bookings;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BPS.Application.Services
{

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingService(
            IBookingRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }


        // ============================================================
        // LOCK SEATS
        // ============================================================

        public async Task<LockSeatsResponseDto> LockSeatsAsync(
            LockSeatsDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.TripId <= 0)
                throw new ArgumentException(
                    "Trip is required.");

            if (dto.TripSeatIds == null ||
                dto.TripSeatIds.Count == 0)
            {
                throw new ArgumentException(
                    "At least one seat is required.");
            }


            // --------------------------------------------------------
            // Get Customer ID from JWT
            // --------------------------------------------------------

            var customerIdClaim =
                _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(
                        ClaimTypes.NameIdentifier)?
                    .Value;

            if (!long.TryParse(
                    customerIdClaim,
                    out var customerId))
            {
                throw new UnauthorizedAccessException(
                    "Customer identity not found.");
            }


            // --------------------------------------------------------
            // Lock seats
            // --------------------------------------------------------

            var seats =
                await _repository.LockSeatsAsync(
                    dto.TripId,
                    dto.TripSeatIds,
                    customerId);

            var seatList =
                seats.ToList();

            if (seatList.Count == 0)
            {
                throw new InvalidOperationException(
                    "No seats were locked.");
            }


            // --------------------------------------------------------
            // Locked Until
            // --------------------------------------------------------

            var lockedUntil =
                seatList
                    .First()
                    .LockedUntil!.Value;


            // --------------------------------------------------------
            // Response
            // --------------------------------------------------------

            return new LockSeatsResponseDto
            {
                TripId = dto.TripId,

                LockedUntil = lockedUntil,

                Seats = seatList
                    .Select(x => new LockedSeatDto
                    {
                        TripSeatId = x.Id,

                        TripId = x.TripId,

                        BusSeatId = x.BusSeatId,

                        SeatNumber = x.SeatNumber,

                        Status = x.Status,

                        LockedUntil =
                            x.LockedUntil!.Value
                    })
                    .ToList()
            };
        }


        // ============================================================
        // CONFIRM BOOKING
        // ============================================================

        public async Task<ConfirmBookingResponseDto>
            ConfirmBookingAsync(
                ConfirmBookingDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));


            // --------------------------------------------------------
            // Validate Trip
            // --------------------------------------------------------

            if (dto.TripId <= 0)
            {
                throw new ArgumentException(
                    "Trip is required.");
            }


            // --------------------------------------------------------
            // Validate passengers
            // --------------------------------------------------------

            if (dto.Passengers == null ||
                dto.Passengers.Count == 0)
            {
                throw new ArgumentException(
                    "At least one passenger is required.");
            }


            // --------------------------------------------------------
            // Validate payment method
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                    dto.PaymentMethod))
            {
                throw new ArgumentException(
                    "Payment method is required.");
            }


            // --------------------------------------------------------
            // Validate passenger information
            // --------------------------------------------------------

            foreach (var passenger in dto.Passengers)
            {
                if (passenger.TripSeatId <= 0)
                {
                    throw new ArgumentException(
                        "Invalid TripSeatId.");
                }

                if (string.IsNullOrWhiteSpace(
                        passenger.PassengerName))
                {
                    throw new ArgumentException(
                        "Passenger name is required.");
                }

                if (string.IsNullOrWhiteSpace(
                        passenger.PassengerPhone))
                {
                    throw new ArgumentException(
                        "Passenger phone is required.");
                }
            }


            // --------------------------------------------------------
            // Duplicate seat validation
            // --------------------------------------------------------

            var duplicateSeat =
                dto.Passengers
                    .GroupBy(x => x.TripSeatId)
                    .Any(g => g.Count() > 1);

            if (duplicateSeat)
            {
                throw new ArgumentException(
                    "Duplicate seats are not allowed.");
            }


            // --------------------------------------------------------
            // Get Customer ID from JWT
            // --------------------------------------------------------

            var customerIdClaim =
                _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(
                        ClaimTypes.NameIdentifier)?
                    .Value;

            if (!long.TryParse(
                    customerIdClaim,
                    out var customerId))
            {
                throw new UnauthorizedAccessException(
                    "Customer identity not found.");
            }


            // --------------------------------------------------------
            // Convert passengers to JSON
            // --------------------------------------------------------

            var passengersJson =
                JsonSerializer.Serialize(
                    dto.Passengers);


            // --------------------------------------------------------
            // Confirm booking
            // --------------------------------------------------------

            var result =
                await _repository.ConfirmBookingAsync(
                    dto.TripId,
                    customerId,
                    dto.PaymentMethod,
                    dto.TransactionId,
                    passengersJson);


            if (result == null)
            {
                throw new InvalidOperationException(
                    "Booking could not be confirmed.");
            }


            // --------------------------------------------------------
            // Booking
            // --------------------------------------------------------

            var booking =
                result.Booking;


            // --------------------------------------------------------
            // Final response
            // --------------------------------------------------------

            return new ConfirmBookingResponseDto
            {
                BookingId = booking.Id,

                PNR = booking.PNR,

                TripId = booking.TripId,

                CustomerId = booking.CustomerId,

                TotalAmount = booking.TotalAmount,

                BookingStatus =
                    (byte)booking.BookingStatus,

                PaymentStatus =
                    (byte)booking.PaymentStatus,

                PaymentMethod =
                    booking.PaymentMethod,

                TransactionId =
                    booking.TransactionId,

                CreatedAt =
                    booking.CreatedAt,

                ConfirmedAt =
                    booking.ConfirmedAt,

                Passengers =
                    result.Passengers,
                FromPlaceName = booking.FromPlaceName,
                ToPlaceName = booking.ToPlaceName,
                TripDate = booking.TripDate,
            };
        }
    }

}
