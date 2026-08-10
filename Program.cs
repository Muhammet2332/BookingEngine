using BookingEngine.Data;
using BookingEngine.Endpoints;
using BookingEngine.Interfaces;
using BookingEngine.Services;

var builder = WebApplication.CreateBuilder(args);

/*// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();*/

var connectionString = builder.Configuration.GetConnectionString("BookingEngine");
builder.Services.AddSqlite<BookingDbContext>(connectionString);

builder.Services.AddScoped<IBookingService, BookingService>();
// builder.Services.AddScoped<IRoomService, RoomService>();
// builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();

var app = builder.Build();

/*// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();*/

app.MapRoomsEndpoints();
app.MapRoomTypeEndpoints();
app.MapBookingsEndpoints();

await app.MigrateDbAsync();

app.Run();
