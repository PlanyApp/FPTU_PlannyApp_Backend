using System;

namespace PlanyApp.Service.Dto.Items
{
    public class TransportationDto : ItemDto
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
    }
} 