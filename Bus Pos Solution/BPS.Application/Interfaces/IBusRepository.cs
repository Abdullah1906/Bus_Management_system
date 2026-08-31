using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IBusRepository
    {
        Task<Bus?> CreateAsync(Bus bus);

        Task<Bus?> GetByIdAsync(int id);

        Task<IEnumerable<Bus>> GetAllAsync();

        Task<bool> UpdateAsync(Bus bus);

        Task<bool> DeleteAsync(int id);

        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive);
    }
}
