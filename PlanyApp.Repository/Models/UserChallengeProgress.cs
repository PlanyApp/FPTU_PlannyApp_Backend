// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanyApp.Repository.Models;

[Table("UserChallengeProgress")]
public partial class UserChallengeProgress
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserChallengeProgressId { get; set; }

    public int UserId { get; set; }

    public int ChallengeId { get; set; }

    [Required]
    [StringLength(50)]
    public string? Status { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? ProofImageId { get; set; }

    public string? VerificationNotes { get; set; }

    public int? PointsEarned { get; set; }

    [ForeignKey("ChallengeId")]
    public virtual Challenge? Challenge { get; set; }

    [ForeignKey("ProofImageId")]
    public virtual Image? ProofImage { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
