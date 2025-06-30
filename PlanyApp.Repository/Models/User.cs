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

    public bool IsActive { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public int? Points { get; set; }

    public virtual ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<UserActivationToken> UserActivationTokens { get; set; } = new List<UserActivationToken>();

    public virtual ICollection<UserChallengeProgress> UserChallengeProgresses { get; set; } = new List<UserChallengeProgress>();

    public virtual ICollection<UserGift> UserGifts { get; set; } = new List<UserGift>();

    public virtual ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
