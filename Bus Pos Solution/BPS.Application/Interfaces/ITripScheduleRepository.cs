using BPS.Application.DTOs.Trips;
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
    }
}
