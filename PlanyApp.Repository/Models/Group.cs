using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Group
{
    public int GroupId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? PlanId { get; set; }

    public DateTime GroupStart { get; set; }

    public DateTime GroupEnd { get; set; }

    public int TotalMember { get; set; }

    public int OwnerId { get; set; }

    public bool IsPrivate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
