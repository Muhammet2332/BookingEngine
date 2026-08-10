using BookingEngine.Data;
using BookingEngine.Dtos;
using BookingEngine.Entities;
using BookingEngine.Interfaces;
using BookingEngine.Mapping;
using Microsoft.EntityFrameworkCore;

namespace BookingEngine.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _db;

    public BookingService(BookingDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync()
    {
        return await _db.Bookings
                        .Select(b => b.ToDto())
                        .ToListAsync();
    }

    public async Task<BookingDto?> GetByIdAsync(int id)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        return booking?.ToDto();
    }

    public async Task<BookingResult<BookingDto>> CreateAsync(CreateBookingDto dto)
    {
        // Checking if a room exists
        var room = await _db.Rooms.FindAsync(dto.RoomId);
        if (room is null)
        {
            return BookingResult<BookingDto>.Fail($"Комната с ID {dto.RoomId} не найдена.");
        }

        // Check if the room is available for the selected dates
        var isOverlap = await _db.Bookings.AnyAsync(b =>
            b.RoomId == dto.RoomId &&
            dto.StartDate < b.EndDate &&
            dto.EndDate > b.StartDate);

        if (isOverlap)
        {
            return BookingResult<BookingDto>.Fail("Комната уже забронирована на выбранные даты.");
        }

        decimal totalPrice = (decimal)(dto.EndDate.DayNumber - dto.StartDate.DayNumber) * room.PricePerNight;

        // Creating a reservation
        Booking booking = dto.ToEntity();
        booking.TotalPrice = totalPrice;
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return BookingResult<BookingDto>.Ok(booking.ToDto());
    }

    public async Task<bool> UpdateAsync(int id, CreateBookingDto dto)
    {
        var existingBooking = await _db.Bookings.FindAsync(id);
        if (existingBooking is null)
            return false;

        _db.Entry(existingBooking).CurrentValues.SetValues(dto.ToEntity(id));
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rowsAffected = await _db.Bookings
                                   .Where(b => b.Id == id)
                                   .ExecuteDeleteAsync();
        return rowsAffected > 0;
    }
}