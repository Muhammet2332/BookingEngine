using BookingEngine.Data;
using BookingEngine.Dtos;
using BookingEngine.Entities;
using BookingEngine.Interfaces;
using BookingEngine.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingEngine.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BookingService(BookingDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    private (string? Username, bool IsAdmin) GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null) return (null, false);

        var username = user.FindFirst(ClaimTypes.Name)?.Value ?? user.FindFirst("sub")?.Value;
        var isAdmin = user.IsInRole("Admin");

        return (username, isAdmin);
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync()
    {
        var (username, isAdmin) = GetCurrentUser();
        var query = _db.Bookings.AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(b => b.CustomerName == username);
        }

        return await query.Select(b => b.ToDto()).ToListAsync();
    }

    public async Task<BookingDto?> GetByIdAsync(int id)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id); 

        var (username, isAdmin) = GetCurrentUser();

        if (!isAdmin && booking.CustomerName != username)
        {
            return null;
        }
        return booking?.ToDto();
    }
    public async Task<BookingResult<BookingDto>> CreateAsync(CreateBookingDto dto)
    {
        // 1. Проверяем существование комнаты
        var room = await _db.Rooms.FindAsync(dto.RoomId);
        if (room is null)
        {
            return BookingResult<BookingDto>.Fail($"Комната с ID {dto.RoomId} не найдена.");
        }

        // 2. Проверяем перекрытие дат
        var isOverlap = await _db.Bookings.AnyAsync(b =>
            b.RoomId == dto.RoomId &&
            dto.StartDate < b.EndDate &&
            dto.EndDate > b.StartDate);

        if (isOverlap)
        {
            return BookingResult<BookingDto>.Fail("Комната уже забронирована на выбранные даты.");
        }

        // 3. Создаем сущность и рассчитываем цену через нашу функцию
        Booking booking = dto.ToEntity();
        booking.TotalPrice = CalculateTotalPrice(dto.StartDate, dto.EndDate, room.PricePerNight);

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return BookingResult<BookingDto>.Ok(booking.ToDto());
    }

    public async Task<bool> UpdateAsync(int id, CreateBookingDto dto)
    {
        // 1. Ищем бронирование
        var existingBooking = await _db.Bookings.FindAsync(id);
        if (existingBooking is null)
            return false;

        // 2. Проверяем права доступа
        var (username, isAdmin) = GetCurrentUser();
        if (!isAdmin && existingBooking.CustomerName != username)
        {
            return false;
        }

        // 3. Получаем комнату для расчета обновленной стоимости
        var room = await _db.Rooms.FindAsync(dto.RoomId);
        if (room is null)
        {
            return false; // Или выбрасывать соответствующее исключение / ошибку
        }

        // 4. Проверяем перекрытие дат (исключая текущее бронирование!)
        var isOverlap = await _db.Bookings.AnyAsync(b =>
            b.Id != id &&
            b.RoomId == dto.RoomId &&
            dto.StartDate < b.EndDate &&
            dto.EndDate > b.StartDate);

        if (isOverlap)
        {
            return false;
        }

        // 5. Переносим значения из DTO в сущность
        _db.Entry(existingBooking).CurrentValues.SetValues(dto.ToEntity(id));

        // 6. Обновляем TotalPrice с помощью той же функции
        existingBooking.TotalPrice = CalculateTotalPrice(dto.StartDate, dto.EndDate, room.PricePerNight);

        await _db.SaveChangesAsync();
        return true;
    }

    private static decimal CalculateTotalPrice(DateOnly startDate, DateOnly endDate, decimal pricePerNight)
    {
        int nights = endDate.DayNumber - startDate.DayNumber;
        return nights > 0 ? nights * pricePerNight : 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rowsAffected = await _db.Bookings
                                   .Where(b => b.Id == id)
                                   .ExecuteDeleteAsync();
        return rowsAffected > 0;
    }
}