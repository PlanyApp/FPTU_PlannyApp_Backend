using System;

namespace PlanyApp.Service.Dto.Items
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 