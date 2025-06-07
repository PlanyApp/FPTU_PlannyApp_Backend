using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class GroupMember
{
    public int GroupId { get; set; }

    public int UserId { get; set; }

    public decimal? CashContributed { get; set; }

    public DateTime JoinedAt { get; set; }

    public string? RoleInGroup { get; set; }
}
