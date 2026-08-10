using Microsoft.EntityFrameworkCore;
using BookingEngine.Entities;

namespace BookingEngine.Data
{
    public class BookingDbContext (DbContextOptions<BookingDbContext> options) : DbContext(options)
    {
        public DbSet<Room> Rooms => Set <Room>();

        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<RoomType> RoomTypes => Set<RoomType>();
    }
}
