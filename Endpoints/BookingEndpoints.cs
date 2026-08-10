using BookingEngine.Dtos;
using BookingEngine.Interfaces;
using BookingEngine.Services;

namespace BookingEngine.Endpoints
{
    public static class BookingEndpoints
    {
        const string GetBookingEndpointName = "GetBooking";

        public static RouteGroupBuilder MapBookingsEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("bookings")
                           .WithParameterValidation();

            // GET /bookings
            group.MapGet("/", async (IBookingService service) =>
                await service.GetAllAsync());

            // GET /bookings/{id}
            group.MapGet("/{id}", async (int id, IBookingService service) =>
            {
                var booking = await service.GetByIdAsync(id);
                return booking is not null ? Results.Ok(booking) : Results.NotFound();
            })
            .WithName(GetBookingEndpointName);

            // POST /bookings
            group.MapPost("/", async (CreateBookingDto newBooking, IBookingService service) =>
            {
                var result = await service.CreateAsync(newBooking);

                if (!result.Success)
                {
                    // Return a 400 Bad Request with a clear reason for the error.
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.CreatedAtRoute(
                    GetBookingEndpointName,
                    new { id = result.Data!.Id },
                    result.Data
                );
            });

            // PUT /bookings/{id}
            group.MapPut("/{id}", async (int id, CreateBookingDto updateBooking, IBookingService service) =>
            {
                var updated = await service.UpdateAsync(id, updateBooking);
                return updated ? Results.NoContent() : Results.NotFound();
            });

            // DELETE /bookings/{id}
            group.MapDelete("/{id}", async (int id, IBookingService service) =>
            {
                var deleted = await service.DeleteAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            });

            return group;
        }
    }
}