namespace BookingEngine.Dtos
{
    public record class BookingDto(
        int Id,
        int RoomId,
        string CustomerName,
        DateOnly StartDate,
        DateOnly EndDate,
        decimal TotalPrice);
}
