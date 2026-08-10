using BookingEngine.Entities;

namespace BookingEngine.Dtos
{
    public record class RoomDto(
        int Id,
        string Number,
        int RoomTypeId,
        string RoomType,
        decimal PricePerNight,
        bool IsAvailable);
}
