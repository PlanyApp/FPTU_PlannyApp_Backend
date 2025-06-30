using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Gift
{
    public int GiftId { get; set; }

    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public int Point { get; set; }

    public virtual ICollection<UserGift> UserGifts { get; set; } = new List<UserGift>();
}
