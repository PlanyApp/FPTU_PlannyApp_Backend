using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Avatar { get; set; }

    public int? RoleId { get; set; }

    public string? GoogleId { get; set; }

    public bool EmailVerified { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<UserActivationToken> UserActivationTokens { get; set; } = new List<UserActivationToken>();
}
