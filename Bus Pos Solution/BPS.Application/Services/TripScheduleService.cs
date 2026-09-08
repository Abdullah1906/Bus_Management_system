using BPS.Application.DTOs.TripSchedules;
using BPS.Application.DTOs.TripSearch;
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

        public async Task<TripScheduleDto> CreateAsync( CreateTripScheduleDto dto)
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


        public async Task<IReadOnlyList<TripScheduleDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllAsync(
                cancellationToken);
        }

        public async Task<TripScheduleDto?> GetByIdAsync(long id,CancellationToken cancellationToken = default)
            {
                if (id <= 0)
                    throw new ArgumentException("Invalid trip id.");

                return await _repository.GetByIdAsync(
                    id,
                    cancellationToken);
            }

        public async Task<bool> UpdateAsync(long id,UpdateTripScheduleDto dto)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Invalid trip id.");

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

            var trip = new Trip
            {
                BusId = dto.BusId,
                RouteId = dto.RouteId,
                TripDate = dto.TripDate.Date,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                Fare = dto.Fare,
                IsActive = dto.IsActive
            };

            return await _repository.UpdateAsync(
                id,
                trip);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Invalid trip id.");

            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ChangeStatusAsync(long id,bool isActive)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Invalid trip id.");

            return await _repository.ChangeStatusAsync(
                id,
                isActive);
        }
        public async Task<IEnumerable<TripSearchResponseDto>> SearchAsync(
        TripSearchDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.FromPlace))
                throw new ArgumentException(
                    "From place is required.");

            if (string.IsNullOrWhiteSpace(request.ToPlace))
                throw new ArgumentException(
                    "To place is required.");

            if (request.FromPlace.Trim()
                .Equals(
                    request.ToPlace.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "From place and To place cannot be the same.");
            }

            if (request.TripDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException(
                    "Trip date cannot be in the past.");

            return await _repository.SearchAsync(
                request.FromPlace.Trim(),
                request.ToPlace.Trim(),
                request.TripDate.Date);
        }
    }
}
