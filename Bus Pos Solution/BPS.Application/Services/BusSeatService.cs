using BPS.Application.DTOs.BusSeats;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Services
{
    public class BusSeatService:IBusSeatService
    {
        private readonly IBusSeatRepository _repository;

        public BusSeatService(
            IBusSeatRepository repository)
        {
            _repository = repository;
        }

        public async Task<BusSeatDto> CreateAsync(
            int busId,
            CreateBusSeatDto dto)
        {
            Validate(dto.SeatNumber,
                dto.RowNumber,
                dto.ColumnNumber);

            var seat = new BusSeat
            {
                BusId = busId,
                SeatNumber = dto.SeatNumber.Trim(),
                RowNumber = dto.RowNumber,
                ColumnNumber = dto.ColumnNumber,
                IsWindow = dto.IsWindow
            };

            var result =
                await _repository.CreateAsync(seat);

            if (result == null)
                throw new InvalidOperationException(
                    "Seat could not be created.");

            return MapToDto(result);
        }

        public async Task<IEnumerable<BusSeatDto>>
            GetByBusIdAsync(int busId)
        {
            var seats =
                await _repository
                    .GetByBusIdAsync(busId);

            return seats.Select(MapToDto);
        }

        public async Task<bool> UpdateAsync(
            long id,
            UpdateBusSeatDto dto)
        {
            Validate(dto.SeatNumber,
                dto.RowNumber,
                dto.ColumnNumber);

            var seat =
                await _repository.GetByIdAsync(id);

            if (seat == null)
                return false;

            seat.SeatNumber =
                dto.SeatNumber.Trim();

            seat.RowNumber =
                dto.RowNumber;

            seat.ColumnNumber =
                dto.ColumnNumber;

            seat.IsWindow =
                dto.IsWindow;

            seat.IsActive =
                dto.IsActive;

            return await _repository
                .UpdateAsync(seat);
        }

        public async Task<bool> DeleteAsync(
            long id)
        {
            return await _repository
                .DeleteAsync(id);
        }

        public async Task<bool> ChangeStatusAsync(
            long id,
            bool isActive)
        {
            return await _repository
                .ChangeStatusAsync(
                    id,
                    isActive);
        }

        private static void Validate(
            string seatNumber,
            int row,
            int column)
        {
            if (string.IsNullOrWhiteSpace(
                seatNumber))
            {
                throw new ArgumentException(
                    "Seat number is required.");
            }

            if (row <= 0)
            {
                throw new ArgumentException(
                    "Row number must be greater than zero.");
            }

            if (column <= 0)
            {
                throw new ArgumentException(
                    "Column number must be greater than zero.");
            }
        }

        private static BusSeatDto MapToDto(
            BusSeat seat)
        {
            return new BusSeatDto
            {
                Id = seat.Id,
                BusId = seat.BusId,
                SeatNumber = seat.SeatNumber,
                RowNumber = seat.RowNumber,
                ColumnNumber = seat.ColumnNumber,
                IsWindow = seat.IsWindow,
                IsActive = seat.IsActive,
                CreatedAt = seat.CreatedAt
            };
        }
    }
}
