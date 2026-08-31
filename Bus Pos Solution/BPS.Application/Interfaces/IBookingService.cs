using BPS.Application.DTOs.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IBookingService
    {
        Task<LockSeatsResponseDto> LockSeatsAsync(
            LockSeatsDto dto);

        Task<ConfirmBookingResponseDto> ConfirmBookingAsync(
        ConfirmBookingDto dto);
    }
}
