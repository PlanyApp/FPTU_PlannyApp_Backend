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

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

  //  public string? Image { get; set; }

    public int? ProvinceId { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Package? Package { get; set; }

    public virtual Province? Province { get; set; }
    public int? ImageS3Id { get; set; }
    public virtual ImageS3? ImageS3 { get; set; }

    public virtual ICollection<UserChallengeProgress> UserChallengeProgresses { get; set; } = new List<UserChallengeProgress>();
}
