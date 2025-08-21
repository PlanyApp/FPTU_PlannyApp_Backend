using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Plan
{
    public int PlanId { get; set; }

    public string Name { get; set; } = null!;

    public int DayCount { get; set; }

    public int NightCount { get; set; }

    public decimal TotalCost { get; set; }

    public int OwnerId { get; set; }

    public string Status { get; set; } = "Draft";

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? ProvinceId { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual User Owner { get; set; } = null!;

    public virtual Province? Province { get; set; }

    public virtual ICollection<PlanList> PlanLists { get; set; } = new List<PlanList>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
