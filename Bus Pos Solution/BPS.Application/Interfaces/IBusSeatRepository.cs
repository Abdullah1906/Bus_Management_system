using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IBusSeatRepository
    {
        Task<BusSeat?> CreateAsync(
            BusSeat seat);

        Task<IEnumerable<BusSeat>> GetByBusIdAsync(
            int busId);

        Task<BusSeat?> GetByIdAsync(
            long id);

        Task<bool> UpdateAsync(
            BusSeat seat);

        Task<bool> DeleteAsync(
            long id);

        Task<bool> ChangeStatusAsync(
            long id,
            bool isActive);
    }
}
