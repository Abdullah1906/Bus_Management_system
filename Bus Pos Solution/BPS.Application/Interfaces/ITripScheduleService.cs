using BPS.Application.DTOs.Trips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface ITripScheduleService
    {
        Task<TripScheduleDto> CreateAsync(
            CreateTripScheduleDto dto);
    }
}
