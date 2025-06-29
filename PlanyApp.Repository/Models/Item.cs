using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string ItemType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Address { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public decimal? Price { get; set; }

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();

    public virtual Hotel? Hotel { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual Place? Place { get; set; }

    public virtual ICollection<PlanList> PlanLists { get; set; } = new List<PlanList>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual Transportation? Transportation { get; set; }
}
