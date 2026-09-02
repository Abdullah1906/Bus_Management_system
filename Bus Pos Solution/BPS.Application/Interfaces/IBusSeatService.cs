using BPS.Application.DTOs.BusSeats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IBusSeatService
    {
        Task<BusSeatDto> CreateAsync(int busId,CreateBusSeatDto dto);
        Task<IEnumerable<BusSeatDto>>GetByBusIdAsync(int busId);
        Task<bool> UpdateAsync(long id, UpdateBusSeatDto dto);
        Task<bool> DeleteAsync(long id);
        Task<bool> ChangeStatusAsync(long id,bool isActive);
    }
}
