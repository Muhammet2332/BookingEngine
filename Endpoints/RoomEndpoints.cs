using BookingEngine.Data;
using BookingEngine.Dtos;
using BookingEngine.Entities;
using BookingEngine.Mapping;
using Microsoft.EntityFrameworkCore;

namespace BookingEngine.Endpoints
{
    public static class RoomEndpoints
    {
        const string GetRoomEndpointName = "GetRoom";
        public static RouteGroupBuilder MapRoomsEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("rooms")
                           .WithParameterValidation();

            // GET /rooms
            group.MapGet("/", async (BookingDbContext dbContext) =>
                await dbContext.Rooms
                               .Include(room => room.RoomType)
                               .Select(room => room.ToDto())
                               .ToListAsync()); 

            // GET /rooms/{id}
            group.MapGet("/{id}", async (int id, BookingDbContext db) =>
            {
                var room = await db.Rooms
                                   .Include(room => room.RoomType)
                                   .FirstOrDefaultAsync(room => room.ID == id);

                return room is not null ? Results.Ok(room.ToDto()) : Results.NotFound();
            })
            .WithName(GetRoomEndpointName);

            // POST /rooms
            group.MapPost("/", async (CreateRoomDto newRoom, BookingDbContext dbContext) =>
            {
                Room room = newRoom.ToEntity();

                dbContext.Rooms.Add(room);
                await dbContext.SaveChangesAsync(); 

                var createdRoom = await dbContext.Rooms
                                                 .Include(r => r.RoomType)
                                                 .FirstOrDefaultAsync(r => r.ID == room.ID);

                return Results.CreatedAtRoute(GetRoomEndpointName, new { id = room.ID }, createdRoom!.ToDto());
            });

            // PUT /rooms/{id}
            group.MapPut("/{id}", async (int id, CreateRoomDto updateRoom, BookingDbContext dbContext) =>
            {
                var ExistingRoom = await dbContext.Rooms.FindAsync(id);
                if (ExistingRoom is null)
                    return Results.NotFound();

                dbContext.Entry(ExistingRoom).CurrentValues.SetValues(updateRoom.ToEntity(id));
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            });

            // DELETE /rooms/{id}
            group.MapDelete("/{id}", async (int id, BookingDbContext dbContext) =>
            {
                await dbContext.Rooms
                               .Where(room => room.ID == id)
                               .ExecuteDeleteAsync();

                return Results.NoContent();
            });


            return group;
        }
    }
}
