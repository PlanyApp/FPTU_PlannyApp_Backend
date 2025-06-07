using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Transportation
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }
}
