using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Rating
{
    public int RatingId { get; set; }

    public int? PlanId { get; set; }

    public int? ItemId { get; set; }

    public int UserId { get; set; }

    public int Rate { get; set; }

    public string? Comment { get; set; }

    public DateTime DatePosted { get; set; }

    public DateTime CreatedAt { get; set; }
}
