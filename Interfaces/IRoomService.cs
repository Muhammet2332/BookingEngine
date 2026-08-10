using BookingEngine.Dtos;
using BookingEngine.Services;

namespace BookingEngine.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllAsync();
        Task<RoomDto?> GetByIdAsync(int id);
        Task<RoomDto?> CreateAsync(CreateRoomDto dto);
        Task<bool> UpdateAsync(int id, CreateRoomDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
