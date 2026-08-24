using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface ITripRepository
    {
        Task<TripRecord?> CreateAsync(
            TripRecord trip);

        Task<TripRecord?> GetByIdAsync(
            long id);

        Task<IEnumerable<TripRecord>> GetAllAsync();
        Task<TripRecord?> UpdateAsync(TripRecord trip);

        Task<bool> DeleteAsync(
            long id,
            string? updatedBy);

    }
}
