# BookingEngine

A REST API for hotel room booking built with ASP.NET Core (.NET 8, Minimal API), using SQLite for storage.

> **Status: work in progress.** The core functionality works, but some planned features are not implemented yet. See [Status and roadmap](#status-and-roadmap) for the current state.

## Features

- Manage room types (`room-types`) and rooms (`rooms`)
- Create, view, update, and delete bookings (`bookings`)
- Date overlap check: a room cannot be booked if it is already taken for the selected period
- Automatic price calculation: number of nights × price per night
- JWT Bearer authentication with role-based access (`Admin` / regular client)
- EF Core migrations are applied automatically on application startup

## Tech stack

| Component | Technology |
|---|---|
| Platform | .NET 8, ASP.NET Core Minimal API |
| Database | SQLite via Entity Framework Core 9 |
| Authentication | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| Validation | `MinimalApis.Extensions` (`WithParameterValidation`) |
| API documentation | Swashbuckle (package installed, see status) |

## Project structure

```
BookingEngine/
├── Program.cs              # Entry point, service and endpoint registration
├── Endpoints/              # Minimal API endpoints (Rooms, RoomTypes, Bookings)
├── Services/               # Booking business logic (BookingService, BookingResult)
├── Interfaces/             # Service interfaces
├── Entities/               # Database models: Room, RoomType, Booking
├── Dtos/                   # Request and response models
├── Mapping/                # Entity <-> DTO extension methods
├── Data/                   # DbContext, migrations, auto-migration on startup
├── *.http                  # Ready-made request examples (Booking, Rooms, Roomtypes)
└── appsettings*.json       # Configuration and connection string
```

## Data model

- **RoomType** — room type (`Name`, `Description`)
- **Room** — room (`Number`, `RoomTypeId`, `PricePerNight`, `IsAvailable`)
- **Booking** — booking (`RoomId`, `CustomerName`, `StartDate`, `EndDate`, `TotalPrice`)

## Quick start

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone <repository-url>
cd BookingEngine
dotnet run
```

The app starts at `http://localhost:5010` (the HTTPS profile uses `https://localhost:7100`). The `BookingEngine.db` database is created automatically and migrations are applied on startup.

The connection string is in `appsettings.json`:

```json
"ConnectionStrings": { "BookingEngine": "Data Source=BookingEngine.db" }
```

## Authentication

For local development, tokens issued with [`dotnet user-jwts`](https://learn.microsoft.com/aspnet/core/security/authentication/jwt-authn) are used. The valid issuer and audiences are configured in `appsettings.Development.json`.

Create an admin token:

```bash
dotnet user-jwts create --name admin --role Admin
```

Create a regular client token:

```bash
dotnet user-jwts create --name user1 --role Client
```

Pass the token in the header: `Authorization: Bearer <token>`.

Access rules:

- A regular user only sees bookings where `CustomerName` matches their name in the token (`name` / `sub` claim)
- `Admin` can view and modify all bookings
- Creating, updating, and deleting rooms and room types is `Admin` only

## API

### Room types — `/room-types`

| Method | Path | Access | Description |
|---|---|---|---|
| GET | `/room-types` | everyone | List room types |
| GET | `/room-types/{id}` | everyone | Get a room type by id |
| POST | `/room-types` | Admin | Create a room type |
| PUT | `/room-types/{id}` | Admin | Update a room type |
| DELETE | `/room-types/{id}` | Admin | Delete a room type |

### Rooms — `/rooms`

| Method | Path | Access | Description |
|---|---|---|---|
| GET | `/rooms` | everyone | List rooms with their type |
| GET | `/rooms/{id}` | everyone | Get a room by id |
| POST | `/rooms` | Admin | Create a room |
| PUT | `/rooms/{id}` | Admin | Update a room |
| DELETE | `/rooms/{id}` | Admin | Delete a room |

### Bookings — `/bookings`

| Method | Path | Description |
|---|---|---|
| GET | `/bookings` | Own bookings (all bookings for Admin) |
| GET | `/bookings/{id}` | Get a booking by id |
| POST | `/bookings` | Create a booking |
| PUT | `/bookings/{id}` | Update a booking |
| DELETE | `/bookings/{id}` | Delete a booking |

Example of creating a booking:

```http
POST /bookings
Content-Type: application/json

{
  "roomId": 1,
  "customerName": "user1",
  "startDate": "2026-09-01",
  "endDate": "2026-09-05"
}
```

If the room is not found or the dates overlap with an existing booking, the API returns `400 Bad Request` with an error description. Ready-made requests for all endpoints are in `Booking.http`, `Rooms.http`, and `Roomtypes.http` (they work in Visual Studio and in VS Code with the REST Client extension).

## Status and roadmap

**Done**

- [x] CRUD for room types, rooms, and bookings
- [x] Date overlap check and price calculation
- [x] JWT authentication and role-based access
- [x] EF Core migrations and SQLite

**In progress / planned**

- [ ] Service layer for rooms and room types (`IRoomService` and `IRoomTypeService` are declared but not implemented yet; the endpoints use `DbContext` directly)
- [ ] Swagger UI setup (the package is installed but not configured in `Program.cs`)
- [ ] More accurate response codes when a booking update fails
- [ ] `IsAvailable` check and tighter permissions for creating bookings
- [ ] Protection against deleting room types and rooms that are still referenced
- [ ] Unit and integration tests
- [ ] CI (GitHub Actions)
- [ ] Search for available rooms for a given period
