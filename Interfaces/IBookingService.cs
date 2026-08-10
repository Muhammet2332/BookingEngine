using BookingEngine.Dtos;
using BookingEngine.Services;

namespace BookingEngine.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllAsync();
        Task<BookingDto?> GetByIdAsync(int id);
        Task<BookingResult<BookingDto>> CreateAsync(CreateBookingDto dto);
        Task<bool> UpdateAsync(int id, CreateBookingDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
