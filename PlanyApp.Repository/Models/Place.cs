using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Place
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public virtual Item Item { get; set; } = null!;
}
