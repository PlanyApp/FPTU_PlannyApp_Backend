using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Province
{
    public int ProvinceId { get; set; }

    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public virtual ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();

    public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();
}
