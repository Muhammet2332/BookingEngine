using BookingEngine.Dtos;
using BookingEngine.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingEngine.Mapping
{
    public static class RoomsMapping
    {
        public static Room ToEntity(this CreateRoomDto room)
        {
            return new Room()
            {
                Number = room.Number,
                RoomTypeId = room.RoomTypeId,
                PricePerNight = room.PricePerNight,
                IsAvailable = room.IsAvailable,
            };
        }

        public static Room ToEntity(this CreateRoomDto room, int id)
        {
            var entity = room.ToEntity();
            entity.ID = id;

            return entity;
        }

        public static RoomDto ToDto(this Room room)
        {
            return new(
                room.ID,
                room.Number,
                room.RoomTypeId,
                room.RoomType!.Name,
                room.PricePerNight,
                room.IsAvailable
                );
        }
    }
}
