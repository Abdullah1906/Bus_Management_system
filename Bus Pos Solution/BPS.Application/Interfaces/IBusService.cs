using BPS.Application.DTOs.Buses;

namespace BPS.Application.Interfaces
{
    public interface IBusService
    {
        Task<BusDto> CreateAsync(
            CreateBusDto dto,
            string? createdBy);

        Task<IEnumerable<BusDto>>
            GetAllAsync();

        Task<BusDto?> GetByIdAsync(
            int id);

        Task<bool> UpdateAsync(
            int id,
            UpdateBusDto dto,
            string? updatedBy);

        Task<bool> DeleteAsync(
            int id);

        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            string? updatedBy);
    }
}
