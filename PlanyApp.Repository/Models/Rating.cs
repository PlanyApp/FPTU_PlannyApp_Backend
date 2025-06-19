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

    public string? ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Plan? Plan { get; set; }

    public virtual User User { get; set; } = null!;
}
