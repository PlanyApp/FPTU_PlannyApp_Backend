using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string ItemType { get; set; } = null!;

    public int? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }
}
