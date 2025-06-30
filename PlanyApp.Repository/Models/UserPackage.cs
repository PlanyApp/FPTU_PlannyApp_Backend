using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class UserPackage
{
    public int UserPackageId { get; set; }

    public int UserId { get; set; }

    public int PackageId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Package Package { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
