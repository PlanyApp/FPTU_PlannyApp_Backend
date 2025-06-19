using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Package
{
    public int PackageId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public int? DurationDays { get; set; }

    public string Type { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
