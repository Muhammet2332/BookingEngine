using Microsoft.EntityFrameworkCore;

namespace BookingEngine.Data
{
    public static class DataExtentions
    {
        public static async Task MigrateDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
