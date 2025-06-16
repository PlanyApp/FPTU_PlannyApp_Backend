using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Challenge
{
    public int ChallengeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? ItemId { get; set; }

    public int? PackageId { get; set; }

    public int? PointsAwarded { get; set; }

    public string? DifficultyLevel { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Package? Package { get; set; }

    public virtual ICollection<UserChallengeProgress> UserChallengeProgresses { get; set; } = new List<UserChallengeProgress>();
}
