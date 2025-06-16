using System;

namespace PlanyApp.Service.Dto.Items
{
    public class HotelDto : ItemDto
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
    }
} 