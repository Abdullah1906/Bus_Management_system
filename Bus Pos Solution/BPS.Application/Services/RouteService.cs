using BPS.Application.DTOs.Routes;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Services
{
    public class RouteService:IRouteService
    {
        private readonly IRouteRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RouteService(
            IRouteRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RouteDto> CreateAsync(
            CreateRouteDto dto)
        {
            Validate(
                dto.FromPlace,
                dto.ToPlace,
                dto.DistanceKm,
                dto.EstimatedMinutes);

            var user =
                _httpContextAccessor
                    .HttpContext?
                    .User;

            var createdBy =
                user?.Identity?.Name;

            var route = new Route
            {
                FromPlace = dto.FromPlace.Trim(),
                ToPlace = dto.ToPlace.Trim(),
                DistanceKm = dto.DistanceKm,
                EstimatedMinutes = dto.EstimatedMinutes,
                CreatedBy = createdBy
            };

            var result =
                await _repository.CreateAsync(route);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Route could not be created.");
            }

            return MapToDto(result);
        }

        public async Task<IEnumerable<RouteDto>>
            GetAllAsync()
        {
            var routes =
                await _repository.GetAllAsync();

            return routes.Select(MapToDto);
        }

        public async Task<RouteDto?> GetByIdAsync(
            int id)
        {
            if (id <= 0)
                return null;

            var route =
                await _repository.GetByIdAsync(id);

            return route == null
                ? null
                : MapToDto(route);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateRouteDto dto)
        {
            Validate(
                dto.FromPlace,
                dto.ToPlace,
                dto.DistanceKm,
                dto.EstimatedMinutes);

            var route =
                await _repository.GetByIdAsync(id);

            if (route == null)
                return false;

            var user =
                _httpContextAccessor
                    .HttpContext?
                    .User;

            route.FromPlace =
                dto.FromPlace.Trim();

            route.ToPlace =
                dto.ToPlace.Trim();

            route.DistanceKm =
                dto.DistanceKm;

            route.EstimatedMinutes =
                dto.EstimatedMinutes;

            route.IsActive =
                dto.IsActive;

            route.UpdatedBy =
                user?.Identity?.Name;

            return await _repository
                .UpdateAsync(route);
        }

        public async Task<bool> DeleteAsync(
            int id)
        {
            if (id <= 0)
                return false;

            return await _repository
                .DeleteAsync(id);
        }

        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive)
        {
            if (id <= 0)
                return false;

            return await _repository
                .ChangeStatusAsync(
                    id,
                    isActive);
        }

        private static void Validate(
            string from,
            string to,
            decimal? distance,
            int? minutes)
        {
            if (string.IsNullOrWhiteSpace(from))
                throw new ArgumentException(
                    "From place is required.");

            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException(
                    "To place is required.");

            if (from.Trim()
                .Equals(
                    to.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "From place and To place cannot be same.");
            }

            if (distance.HasValue &&
                distance.Value < 0)
            {
                throw new ArgumentException(
                    "Distance cannot be negative.");
            }

            if (minutes.HasValue &&
                minutes.Value <= 0)
            {
                throw new ArgumentException(
                    "Estimated minutes must be greater than zero.");
            }
        }

        private static RouteDto MapToDto(
            Route route)
        {
            return new RouteDto
            {
                Id = route.Id,
                FromPlace = route.FromPlace,
                ToPlace = route.ToPlace,
                DistanceKm = route.DistanceKm,
                EstimatedMinutes = route.EstimatedMinutes,
                IsActive = route.IsActive,
                CreatedAt = route.CreatedAt,
                UpdatedAt = route.UpdatedAt
            };
        }
    }
}
