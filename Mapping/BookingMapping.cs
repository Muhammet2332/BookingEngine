using BookingEngine.Dtos;
using BookingEngine.Entities;

namespace BookingEngine.Mapping
{
    public static class BookingMapping
    {
        public static Booking ToEntity(this CreateBookingDto booking)
        {
            return new Booking()
            {
                RoomId = booking.RoomId,
                CustomerName = booking.CustomerName,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
            };
        }

        public static Booking ToEntity(this CreateBookingDto booking, int id)
        {
            var entity = booking.ToEntity();
            entity.Id = id;

            return entity;
        }

        public static BookingDto ToDto(this Booking booking)
        {
            return new(
                booking.Id,
                booking.RoomId,
                booking.CustomerName,
                booking.StartDate,
                booking.EndDate,
                booking.TotalPrice);
        }
    }
}
