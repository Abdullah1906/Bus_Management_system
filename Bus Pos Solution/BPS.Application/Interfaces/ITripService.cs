using BPS.Application.DTOs.Trips;

namespace BPS.Application.Interfaces
{
    public interface ITripService
    {
        Task<TripDto> CreateAsync(
            CreateTripDto dto);

        Task<TripDto?> GetByIdAsync(
            long id);

        Task<IEnumerable<TripDto>> GetAllAsync();

        Task<TripDto?> UpdateAsync(long id,UpdateTripDto dto);

        Task<bool> DeleteAsync(
            long id);
    }
}
