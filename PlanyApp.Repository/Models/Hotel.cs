using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Hotel
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public TimeOnly? CheckInTime { get; set; }

    public TimeOnly? CheckOutTime { get; set; }

    public virtual Item Item { get; set; } = null!;
}
