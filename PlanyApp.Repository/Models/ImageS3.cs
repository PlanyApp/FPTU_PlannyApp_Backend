using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class ImageS3
{
    public int ImageS3id { get; set; }

    public string ReferenceType { get; set; } = null!;

    public int ReferenceId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? ContentType { get; set; }

    public int? FileSizeKb { get; set; }

    public bool? IsPrimary { get; set; }

    public string? Caption { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UserChallengeProgress> UserChallengeProgresses { get; set; } = new List<UserChallengeProgress>();
}
