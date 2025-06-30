using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class UserGift
{
    public int UserGiftId { get; set; }

    public string Code { get; set; } = null!;

    public int UserId { get; set; }

    public int GiftId { get; set; }

    public DateTime? RedeemedAt { get; set; }

    public virtual Gift Gift { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
