// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PlanyApp.Repository.Models;

[Table("GroupMembers")]
[PrimaryKey("GroupId", "UserId")]
public partial class GroupMember
{
    [ForeignKey("Group")]
    public int GroupId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? CashContributed { get; set; }

    public DateTime JoinedAt { get; set; }

    [StringLength(50)]
    public string? RoleInGroup { get; set; }

    public virtual Group? Group { get; set; }

    public virtual User? User { get; set; }
}
