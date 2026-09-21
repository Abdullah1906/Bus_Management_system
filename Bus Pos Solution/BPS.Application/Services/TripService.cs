using BPS.Application.DTOs.Common;
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


        public async Task<List<TripDto>> CreateAsync(CreateTripDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.PlaceIds == null || dto.PlaceIds.Count == 0)
                throw new ArgumentException(
                    "At least one place is required.");

            if (dto.PlaceIds.Any(x => x <= 0))
                throw new ArgumentException(
                    "Invalid place selected.");

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

            var email =
                _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.Email)?
                    .Value;

            var trip = new MutipleTripRecord
            {
                PlaceIds = dto.PlaceIds,

                TripDate = dto.TripDate.Date,

                TipStatus = dto.TipStatus,

                TipAmount = dto.TipAmount,
                CreatedBy = email
            };

            var result = await _tripRepository.CreateMultipleAsync(trip);

            if (result == null || result.Count == 0)
            {
                throw new InvalidOperationException(
                    "Trips could not be created.");
            }

            return result
                .Select(MapToDto)
                .ToList();
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

        public async Task<PagedResult<TripDto>> GetPagedAsync(TripPagedRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(
                    nameof(request));

            var page =
                request.Page < 1
                    ? 1
                    : request.Page;

            var pageSize =
                request.PageSize < 1
                    ? 10
                    : request.PageSize;

            // Maximum page size
            if (pageSize > 100)
                pageSize = 100;

            var result =
                await _tripRepository.GetPagedAsync(
                    request.Search,
                    page,
                    pageSize);

            return new PagedResult<TripDto>
            {
                Items = result.Items
                    .Select(MapToDto)
                    .ToList(),

                Page = result.Page,

                PageSize = result.PageSize,

                TotalCount = result.TotalCount
            };
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
