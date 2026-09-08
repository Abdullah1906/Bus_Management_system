using BPS.Application.DTOs.TripSchedules;
using BPS.Application.DTOs.TripSearch;
using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface ITripScheduleRepository
    {
        Task<Trip?> CreateScheduleAsync(Trip trip);
        Task<IReadOnlyList<TripScheduleDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TripScheduleDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            long id,
            Trip trip,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            long id,
            CancellationToken cancellationToken = default);

        Task<bool> ChangeStatusAsync(
            long id,
            bool isActive,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<TripSearchResponseDto>> SearchAsync(
          string fromPlace,
          string toPlace,
          DateTime tripDate);
    }
}
