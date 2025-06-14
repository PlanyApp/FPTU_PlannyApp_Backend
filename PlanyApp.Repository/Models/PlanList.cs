﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class PlanList
{
    public string PlanListId { get; set; }

    public string PlanId { get; set; }

    public int ItemNo { get; set; }

    public string ItemId { get; set; }

    public int DayNumber { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string Notes { get; set; }

    public decimal? Price { get; set; }

    public virtual Item Item { get; set; }

    public virtual Plan Plan { get; set; }
}