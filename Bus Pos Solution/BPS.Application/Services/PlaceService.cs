using BPS.Application.DTOs.Places;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;


namespace BPS.Application.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PlaceService(
            IPlaceRepository placeRepository, IHttpContextAccessor httpContextAccessor)
        {
            _placeRepository = placeRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        // GET ALL
        public async Task<IEnumerable<PlaceDto>> GetAllAsync()
        {
            var places =
                await _placeRepository.GetAllAsync();

            return places.Select(MapToDto);
        }


        // GET BY ID
        public async Task<PlaceDto?> GetByIdAsync(int id)
        {
            var place =
                await _placeRepository.GetByIdAsync(id);

            if (place == null)
                return null;

            return MapToDto(place);
        }


        // CREATE
        public async Task<int> CreateAsync(
            CreatePlaceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PlaceName))
                throw new ArgumentException(
                    "Place name is required.");

            if (dto.PricePerTrip <= 0)
                throw new ArgumentException(
                    "Price per trip must be greater than zero.");

            var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

            var place = new Place
            {
                PlaceName = dto.PlaceName.Trim(),

                PricePerTrip = dto.PricePerTrip,

                IsActive = true,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = email

            };

            return await _placeRepository.CreateAsync(place);
        }


        // UPDATE
        public async Task<bool> UpdateAsync(
            int id,
            UpdatePlaceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PlaceName))
                throw new ArgumentException(
                    "Place name is required.");

            if (dto.PricePerTrip <= 0)
                throw new ArgumentException(
                    "Price per trip must be greater than zero.");

            

            var existingPlace =
                await _placeRepository.GetByIdAsync(id);

            if (existingPlace == null)
                return false;

            var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

            existingPlace.PlaceName =
                dto.PlaceName.Trim();

            existingPlace.PricePerTrip =
                dto.PricePerTrip;

            existingPlace.IsActive =
                dto.IsActive;

            existingPlace.UpdatedAt =
                DateTime.UtcNow;
            existingPlace.UpdatedBy =
               email;

            return await _placeRepository
                .UpdateAsync(existingPlace);
        }


        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var existingPlace =
                await _placeRepository.GetByIdAsync(id);

            if (existingPlace == null)
                return false;

            return await _placeRepository
                .DeleteAsync(id);
        }


        // ENTITY → DTO
        private static PlaceDto MapToDto(
            Place place)
        {
            return new PlaceDto
            {
                Id = place.Id,

                PlaceName = place.PlaceName,

                PricePerTrip = place.PricePerTrip,

                IsActive = place.IsActive,

                UpdateAt =
                    place.UpdatedAt
                    ?? place.CreatedAt
            };
        }
    }

}   
