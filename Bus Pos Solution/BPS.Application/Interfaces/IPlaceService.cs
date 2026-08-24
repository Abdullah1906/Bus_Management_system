using BPS.Application.DTOs.Places;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IPlaceService
    {
        Task<IEnumerable<PlaceDto>> GetAllAsync();

        Task<PlaceDto?> GetByIdAsync(int id);

        Task<int> CreateAsync(CreatePlaceDto dto);

        Task<bool> UpdateAsync(
            int id,
            UpdatePlaceDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
