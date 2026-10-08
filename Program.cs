using BookingEngine.Data;
using BookingEngine.Endpoints;
using BookingEngine.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BookingEngine.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BookingEngine");
builder.Services.AddSqlite<BookingDbContext>(connectionString);

builder.Services.AddScoped<IBookingService, BookingService>();
// builder.Services.AddScoped<IRoomService, RoomService>();
// builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();

builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapRoomsEndpoints();
app.MapRoomTypeEndpoints();
app.MapBookingsEndpoints();

await app.MigrateDbAsync();

app.Run();
