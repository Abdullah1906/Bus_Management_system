using BPS.Application.DTOs.TripSchedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface ITripScheduleService
    {
        Task<TripScheduleDto> CreateAsync(CreateTripScheduleDto dto);

        Task<IReadOnlyList<TripScheduleDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TripScheduleDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            long id,
            UpdateTripScheduleDto dto);

        Task<bool> DeleteAsync(
            long id);

        Task<bool> ChangeStatusAsync(
            long id,
            bool isActive);
    }
}
