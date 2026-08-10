namespace BookingEngine.Entities
{
    public class Room
    {
        public int ID { get; set; }
        public string Number { get; set; } = string.Empty;
        public int RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
