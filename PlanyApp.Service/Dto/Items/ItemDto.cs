using System;

namespace PlanyApp.Service.Dto.Items
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 