using BookingEngine.Dtos;
using BookingEngine.Entities;

namespace BookingEngine.Mapping
{
    public static class RoomTypeMapping
    {
        public static RoomType ToEntity(this CreateRoomTypeDto type) 
        {
            return new RoomType()
            {
                Name = type.Name,
                Description = type.Description
            };
        }
        public static RoomType ToEntity(this CreateRoomTypeDto type, int id)
        {
            var entity = type.ToEntity();
            entity.Id = id;

            return entity;
        }

        public static RoomTypeDto ToTypeDto(this RoomType type)
        {
            return new(type.Id, type.Name, type.Description);
        }
    }
}
