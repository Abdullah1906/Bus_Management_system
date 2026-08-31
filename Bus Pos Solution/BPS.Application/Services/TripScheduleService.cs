using BPS.Application.DTOs.Trips;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Services
{
    public class TripScheduleService
    : ITripScheduleService
    {
        private readonly ITripScheduleRepository _repository;

        private readonly IHttpContextAccessor
            _httpContextAccessor;

        public TripScheduleService(
            ITripScheduleRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor =
                httpContextAccessor;
        }

        public async Task<TripScheduleDto> CreateAsync(
            CreateTripScheduleDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(
                    nameof(dto));

            if (dto.BusId <= 0)
                throw new ArgumentException(
                    "Bus is required.");

            if (dto.RouteId <= 0)
                throw new ArgumentException(
                    "Route is required.");

            if (dto.TripDate == default)
                throw new ArgumentException(
                    "Trip date is required.");

            if (dto.Fare < 0)
                throw new ArgumentException(
                    "Fare cannot be negative.");

            var email =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.Email)?
                    .Value;

            var trip = new Trip
            {
                BusId = dto.BusId,

                RouteId = dto.RouteId,

                TripDate = dto.TripDate.Date,

                DepartureTime =
                    dto.DepartureTime,

                ArrivalTime =
                    dto.ArrivalTime,

                Fare = dto.Fare,

                CreatedBy = email
            };

            var result =
                await _repository
                    .CreateScheduleAsync(trip);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Trip could not be created.");
            }

            return new TripScheduleDto
            {
                Id = result.Id,
                BusId = result.BusId,
                RouteId = result.RouteId,
                TripDate = result.TripDate,
                DepartureTime = result.DepartureTime,
                ArrivalTime = result.ArrivalTime,
                Fare = result.Fare,
                IsActive = result.IsActive,
                CreatedAt = result.CreatedAt,
                CreatedBy = result.CreatedBy
            };
        }
    }
}
