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
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TripService(
            ITripRepository tripRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tripRepository = tripRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<TripDto> CreateAsync(
            CreateTripDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(
                    nameof(dto));

            if (dto.PlaceId <= 0)
                throw new ArgumentException(
                    "Place is required.");

            if (dto.TripDate == default)
                throw new ArgumentException(
                    "Trip date is required.");

            if (dto.TipAmount < 0)
                throw new ArgumentException(
                    "Tip amount cannot be negative.");

            // Tip OFF
            if (!dto.TipStatus)
            {
                dto.TipAmount = 0;
            }
            var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            var trip = new TripRecord
            {
                PlaceId = dto.PlaceId,

                TripDate = dto.TripDate.Date,

                TipStatus = dto.TipStatus,

                TipAmount = dto.TipAmount,
                CreatedBy = email
            };

            var result =
                await _tripRepository
                    .CreateAsync(trip);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Trip could not be created.");
            }

            return MapToDto(result);
        }


        public async Task<TripDto?> GetByIdAsync(
            long id)
        {
            if (id <= 0)
                return null;

            var trip =
                await _tripRepository
                    .GetByIdAsync(id);

            if (trip == null)
                return null;

            return MapToDto(trip);
        }


        public async Task<IEnumerable<TripDto>> GetAllAsync()
        {
            var trips =
                await _tripRepository
                    .GetAllAsync();

            return trips.Select(MapToDto);
        }

        // UPDATE
        public async Task<TripDto?> UpdateAsync(
            long id,
            UpdateTripDto dto)
        {
            if (id <= 0)
                return null;

            if (dto == null)
                throw new ArgumentNullException(
                    nameof(dto));

            if (dto.PlaceId <= 0)
                throw new ArgumentException(
                    "Place is required.");

            if (dto.TripDate == default)
                throw new ArgumentException(
                    "Trip date is required.");

            if (dto.TipAmount < 0)
                throw new ArgumentException(
                    "Tip amount cannot be negative.");

            // Tip OFF = Tip 0
            if (!dto.TipStatus)
            {
                dto.TipAmount = 0;
            }

            // Check existing trip
            var existingTrip =
                await _tripRepository
                    .GetByIdAsync(id);

            if (existingTrip == null)
                return null;

            var email =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.Email)?
                    .Value;

            existingTrip.PlaceId =
                dto.PlaceId;

            existingTrip.TripDate =
                dto.TripDate.Date;

            existingTrip.TipStatus =
                dto.TipStatus;

            existingTrip.TipAmount =
                dto.TipAmount;

            existingTrip.UpdatedBy =
                email;

            var result =
                await _tripRepository
                    .UpdateAsync(existingTrip);

            if (result == null)
                return null;

            return MapToDto(result);
        }


        // DELETE
        public async Task<bool> DeleteAsync(
            long id)
        {
            if (id <= 0)
                return false;

            var email =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.Email)?
                    .Value;

            return await _tripRepository
                .DeleteAsync(id, email);
        }


        private static TripDto MapToDto(
            TripRecord trip)
        {
            return new TripDto
            {
                Id = trip.Id,

                PlaceId = trip.PlaceId,
                PlaceName =trip.PlaceName,

                TripDate = trip.TripDate,

                TipStatus = trip.TipStatus,

                TipAmount = trip.TipAmount,

                Price = trip.Price,

                Total = trip.Total
            };
        }
    }    
}
