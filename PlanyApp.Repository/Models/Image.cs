using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanyApp.Repository.Models;

[Table("Images")]
public partial class Image
{
    public int ImageId { get; set; }

    public string ReferenceType { get; set; } = null!;

    public int ReferenceId { get; set; }

    public byte[] ImageData { get; set; } = null!;

    public string? ContentType { get; set; }

    public int? FileSizeKb { get; set; }

    public bool? IsPrimary { get; set; }

    public string? Caption { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Item Reference { get; set; } = null!;

    public virtual ICollection<UserChallengeProgress> UserChallengeProgresses { get; set; } = new List<UserChallengeProgress>();
}
