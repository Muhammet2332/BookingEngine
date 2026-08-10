using BookingEngine.Data;
using BookingEngine.Dtos;
using BookingEngine.Entities;
using BookingEngine.Mapping;
using Microsoft.EntityFrameworkCore;

namespace BookingEngine.Endpoints
{
    public static class RoomTypeEndpoints
    {
        const string GetRoomTypeEndpointName = "GetRoomType";
        public static RouteGroupBuilder MapRoomTypeEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("room-types");

            // GET /roomt-ypes
            group.MapGet("/", async (BookingDbContext dbContext) =>
                await dbContext.RoomTypes
                               .Select(type => type.ToTypeDto())
                               .ToListAsync());

            // GET /room-types/{id}
            group.MapGet("/{id}", async (int id, BookingDbContext db) =>
            {
                var type = await db.RoomTypes.FindAsync(id);
                return type is not null ? Results.Ok(type.ToTypeDto()) : Results.NotFound();
            })
            .WithName(GetRoomTypeEndpointName);

            // POST /room-types
            group.MapPost("/", async (CreateRoomTypeDto newType, BookingDbContext dbContext) =>
            {
                RoomType type = newType.ToEntity();
                dbContext.RoomTypes.Add(type);
                await dbContext.SaveChangesAsync();
                return Results.CreatedAtRoute(GetRoomTypeEndpointName, new { id = type.Id }, type.ToTypeDto());
            });

            // PUT /room-types/{id}
            group.MapPut("/{id}", async (int id, CreateRoomTypeDto updateRoomType, BookingDbContext dbContext) =>
            {
                var ExistingRoomType = await dbContext.RoomTypes.FindAsync(id);
                if (ExistingRoomType is null)
                    return Results.NotFound();

                dbContext.Entry(ExistingRoomType).CurrentValues.SetValues(updateRoomType.ToEntity(id));
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            });

            // DELETE /roomtypes/{id}
            group.MapDelete("/{id}", async (int id, BookingDbContext dbContext) =>
            {
                await dbContext.RoomTypes
                               .Where(type => type.Id == id)
                               .ExecuteDeleteAsync();

                return Results.NoContent();
            });

            return group;
        }
    }
}
