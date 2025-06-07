using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Plan
{
    public int PlanId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public int DayCount { get; set; }

    public int NightCount { get; set; }

    public decimal TotalCost { get; set; }

    public int OwnerId { get; set; }

    public string Status { get; set; } = null!;

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
