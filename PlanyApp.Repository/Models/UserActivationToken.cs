﻿using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class UserActivationToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
