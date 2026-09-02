using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IRouteRepository
    {
        Task<Route?> CreateAsync(
            Route route);

        Task<Route?> GetByIdAsync(
            int id);

        Task<IEnumerable<Route>> GetAllAsync();

        Task<bool> UpdateAsync(
            Route route);

        Task<bool> DeleteAsync(
            int id);

        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive);
    }
}
