using BPS.Application.DTOs.Places;
using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IPlaceRepository
    {
        Task<IEnumerable<Place>> GetAllAsync();

        Task<Place?> GetByIdAsync(int id);

        Task<int> CreateAsync(Place place);

        Task<bool> UpdateAsync(Place place);

        Task<bool> DeleteAsync(int id);
    }
}
