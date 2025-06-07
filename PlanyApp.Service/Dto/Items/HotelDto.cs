using System;

namespace PlanyApp.Service.Dto.Items
{
    public class HotelDto : ItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
    }
} 