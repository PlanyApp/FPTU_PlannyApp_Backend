using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class UserChallengeProgress
{
    public int UserChallengeProgressId { get; set; }

    public int UserId { get; set; }

    public int ChallengeId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? ProofImageId { get; set; }

    public string? VerificationNotes { get; set; }

    public int? PointsEarned { get; set; }

    public int? GroupId { get; set; }

    public int? UserPackageId { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;

    public virtual Group? Group { get; set; }

    public virtual ImageS3? ProofImage { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual UserPackage? UserPackage { get; set; }
}
