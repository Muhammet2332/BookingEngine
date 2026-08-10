namespace BookingEngine.Dtos
{
    public record class CreateRoomDto(
        string Number,
        int RoomTypeId,
        decimal PricePerNight,
        bool IsAvailable);
}
