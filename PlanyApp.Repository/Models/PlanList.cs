using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class PlanList
{
    public int PlanListId { get; set; }

    public int PlanId { get; set; }

    public int ItemNo { get; set; }

    public int ItemId { get; set; }

    public int DayNumber { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Notes { get; set; }

    public decimal? Price { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Plan Plan { get; set; } = null!;
}
