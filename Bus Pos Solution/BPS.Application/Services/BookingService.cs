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

        private readonly IHttpContextAccessor
            _httpContextAccessor;

        public BookingService(
            IBookingRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;

            _httpContextAccessor =
                httpContextAccessor;
        }

        public async Task<LockSeatsResponseDto> LockSeatsAsync(
            LockSeatsDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(
                    nameof(dto));

            if (dto.TripId <= 0)
                throw new ArgumentException(
                    "Trip is required.");

            if (dto.TripSeatIds == null ||
                dto.TripSeatIds.Count == 0)
            {
                throw new ArgumentException(
                    "At least one seat is required.");
            }

            var customerIdClaim =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

            if (!long.TryParse(
                    customerIdClaim,
                    out var customerId))
            {
                throw new UnauthorizedAccessException(
                    "Customer identity not found.");
            }

            var seats =
                await _repository.LockSeatsAsync(
                    dto.TripId,
                    dto.TripSeatIds,
                    customerId);

            var seatList = seats.ToList();

            if (seatList.Count == 0)
            {
                throw new InvalidOperationException(
                    "No seats were locked.");
            }

            var lockedUntil =
                seatList.First().LockedUntil!.Value;

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



        public async Task<ConfirmBookingResponseDto>
        ConfirmBookingAsync(
            ConfirmBookingDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(
                    nameof(dto));

            if (dto.TripId <= 0)
                throw new ArgumentException(
                    "Trip is required.");

            if (dto.Passengers == null ||
                dto.Passengers.Count == 0)
            {
                throw new ArgumentException(
                    "At least one passenger is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.PaymentMethod))
            {
                throw new ArgumentException(
                    "Payment method is required.");
            }


         
            // Validate passengers
         

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


            
            // Prevent duplicate TripSeatId
            

            var duplicateSeat =
                dto.Passengers
                    .GroupBy(x => x.TripSeatId)
                    .Any(g => g.Count() > 1);

            if (duplicateSeat)
            {
                throw new ArgumentException(
                    "Duplicate seats are not allowed.");
            }



            // Get CustomerId from JWT


            var customerIdClaim =
                _httpContextAccessor
                    .HttpContext?
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



            // Convert passengers to JSON


            var passengersJson =
                JsonSerializer.Serialize(
                    dto.Passengers);



            // Call repository


            var booking =
                await _repository
                    .ConfirmBookingAsync(
                        dto.TripId,
                        customerId,
                        dto.PaymentMethod,
                        dto.TransactionId,
                        passengersJson);


            if (booking == null)
            {
                throw new InvalidOperationException(
                    "Booking could not be confirmed.");
            }


            // Return response


        return new ConfirmBookingResponseDto
        {
            BookingId = booking.Id,

            PNR = booking.PNR,

            TripId = booking.TripId,

            CustomerId = booking.CustomerId,

            TotalAmount =
                booking.TotalAmount,

            BookingStatus =
                booking.BookingStatus,

            PaymentStatus = 2,

            PaymentMethod =
                dto.PaymentMethod,

            TransactionId =
            dto.TransactionId,

            CreatedAt =
                booking.CreatedAt,

            ConfirmedAt =
                booking.ConfirmedAt,

            Passengers =
                dto.Passengers
                    .Select(x => new ConfirmedPassengerDto
                    {
                        TripSeatId =
                            x.TripSeatId,

                        PassengerName =
                            x.PassengerName,

                        PassengerPhone =
                            x.PassengerPhone,

                        PassengerNID =
                            x.PassengerNID
                    })
                    .ToList()
        };
        }
    }
}
