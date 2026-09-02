using BPS.Application.DTOs.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IRouteService
    {
        Task<RouteDto> CreateAsync(CreateRouteDto dto);
        Task<IEnumerable<RouteDto>>GetAllAsync();
        Task<RouteDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateRouteDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id, bool isActive);
    }
}
