using BPS.Application.DTOs.Buses;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Services
{
    public class BusService:IBusService
    {
        private readonly IBusRepository _repository;

        public BusService(
            IBusRepository repository)
        {
            _repository = repository;
        }

        public async Task<BusDto> CreateAsync(
            CreateBusDto dto,
            string? createdBy)
        {
            if (string.IsNullOrWhiteSpace(dto.BusName))
                throw new ArgumentException(
                    "Bus name is required.");

            if (string.IsNullOrWhiteSpace(dto.BusNumber))
                throw new ArgumentException(
                    "Bus number is required.");

            if (dto.TotalSeats <= 0)
                throw new ArgumentException(
                    "Total seats must be greater than zero.");

            var bus = new Bus
            {
                BusName = dto.BusName.Trim(),

                BusNumber = dto.BusNumber.Trim(),

                TotalSeats = dto.TotalSeats,

                CreatedBy = createdBy
            };

            var result =
                await _repository.CreateAsync(bus);

            if (result == null)
                throw new InvalidOperationException(
                    "Bus could not be created.");

            return MapToDto(result);
        }

        public async Task<IEnumerable<BusDto>>
            GetAllAsync()
        {
            var buses =
                await _repository.GetAllAsync();

            return buses.Select(MapToDto);
        }

        public async Task<BusDto?> GetByIdAsync(
            int id)
        {
            if (id <= 0)
                return null;

            var bus =
                await _repository.GetByIdAsync(id);

            return bus == null
                ? null
                : MapToDto(bus);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateBusDto dto,
            string? updatedBy)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Invalid bus id.");

            if (string.IsNullOrWhiteSpace(dto.BusName))
                throw new ArgumentException(
                    "Bus name is required.");

            if (string.IsNullOrWhiteSpace(dto.BusNumber))
                throw new ArgumentException(
                    "Bus number is required.");

            if (dto.TotalSeats <= 0)
                throw new ArgumentException(
                    "Total seats must be greater than zero.");

            var bus = new Bus
            {
                Id = id,

                BusName = dto.BusName.Trim(),

                BusNumber = dto.BusNumber.Trim(),

                TotalSeats = dto.TotalSeats,

                IsActive = dto.IsActive,

                UpdatedBy = updatedBy
            };

            return await _repository.UpdateAsync(bus);
        }

        public async Task<bool> DeleteAsync(
            int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Invalid bus id.");

            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            string? updatedBy)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Invalid bus id.");

            return await _repository.ChangeStatusAsync(
                id,
                isActive);
        }

        private static BusDto MapToDto(Bus bus)
        {
            return new BusDto
            {
                Id = bus.Id,

                BusName = bus.BusName,

                BusNumber = bus.BusNumber,

                TotalSeats = bus.TotalSeats,

                IsActive = bus.IsActive,

                CreatedAt = bus.CreatedAt,

                UpdatedAt = bus.UpdatedAt
            };
        }
    }
}
