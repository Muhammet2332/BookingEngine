using BookingEngine.Dtos;

namespace BookingEngine.Interfaces
{
    public interface IRoomTypeService
    {
        Task<IEnumerable<RoomTypeDto>> GetAllAsync();
        Task<RoomTypeDto?> GetByIdAsync(int id);
        Task<RoomTypeDto?> CreateAsync(CreateRoomTypeDto dto);
        Task<bool> UpdateAsync(int id, CreateRoomTypeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
