using System;

namespace PlanyApp.Service.Dto.Items
{
    public class PlaceDto : ItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
    }
} 