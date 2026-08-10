namespace BookingEngine.Dtos
{
    public record class CreateBookingDto(
        int RoomId,
        string CustomerName,
        DateOnly StartDate,
        DateOnly EndDate);
}
