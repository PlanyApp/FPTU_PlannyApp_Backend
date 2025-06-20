﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PlanyApp.Repository.Models;

namespace PlanyApp.Repository.Context;

public partial class PlanyDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public PlanyDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public PlanyDbContext(DbContextOptions<PlanyDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Challenge> Challenges { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<PlanList> PlanLists { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transportation> Transportations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivationToken> UserActivationTokens { get; set; }

    public virtual DbSet<UserChallengeProgress> UserChallengeProgresses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is missing in environment variables");
            }
            optionsBuilder.UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.ChallengeId).HasName("PK__Challeng__C7AC8128F8DEDA31");

            entity.ToTable(tb => tb.HasTrigger("trg_Challenges_Update"));

            entity.Property(e => e.ChallengeId).HasColumnName("ChallengeID");
            entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");
            entity.Property(e => e.Description).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.DifficultyLevel)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.PackageId).HasColumnName("PackageID");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Challenges)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK_Challenges_Users");

            entity.HasOne(d => d.Item).WithMany(p => p.Challenges)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK_Challenges_Items");

            entity.HasOne(d => d.Package).WithMany(p => p.Challenges)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_Challenges_Packages");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__149AF30A558FF80E");

            entity.ToTable(tb => tb.HasTrigger("trg_Groups_Update"));

            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.Description).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");

            /*entity.HasOne(d => d.GroupPackageNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.GroupPackage)
                .HasConstraintName("FK_Groups_Packages");*/

            entity.HasOne(d => d.Owner).WithMany(p => p.Groups)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Groups_Owner");

            entity.HasOne(d => d.Plan).WithMany(p => p.Groups)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_Groups_Plans");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.UserId }).HasName("PK__GroupMem__C5E27FC0BCFB2BE3");

            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CashContributed).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RoleInGroup)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_GroupMembers_Groups");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_GroupMembers_Users");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Hotels__727E83EBDD8311A4");

            entity.Property(e => e.ItemId)
                .ValueGeneratedNever()
                .HasColumnName("ItemID");
            entity.Property(e => e.Address).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");

            entity.HasOne(d => d.Item).WithOne(p => p.Hotel)
                .HasForeignKey<Hotel>(d => d.ItemId)
                .HasConstraintName("FK_Hotels_Items");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F4EC93BB9B65");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.Caption).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.ContentType)
                .HasMaxLength(100)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.FileSizeKb).HasColumnName("FileSizeKB");
            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");

            entity.HasOne(d => d.Reference).WithMany(p => p.Images)
                .HasForeignKey(d => d.ReferenceId)
                .HasConstraintName("FK_Images_Items");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD598C262CB");

            entity.HasIndex(e => e.ReferenceCode, "IX_Invoices_ReferenceCode").IsUnique();

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FinalAmount)
                .HasComputedColumnSql("([Amount]-[Discount])", false)
                .HasColumnType("decimal(19, 2)");
            entity.Property(e => e.Notes).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(100)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.ReferenceCode).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8")
                .HasColumnName("TransactionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Package).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_Invoices_Packages");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Users");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Items__727E83EB9B2DA26E");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("tr_Items_UpdatedAt");
                    tb.HasTrigger("trg_Items_Update");
                });

            entity.HasIndex(e => e.IsActive, "IX_Items_IsActive").HasFilter("([IsActive]=(1))");

            entity.HasIndex(e => e.ItemType, "IX_Items_ItemType");

            entity.HasIndex(e => new { e.Latitude, e.Longitude }, "IX_Items_Location").HasFilter("([Latitude] IS NOT NULL AND [Longitude] IS NOT NULL)");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ItemType)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Packages__322035EC24940879");

            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.Description).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Places__727E83EB895695E7");

            entity.Property(e => e.ItemId)
                .ValueGeneratedNever()
                .HasColumnName("ItemID");
            entity.Property(e => e.Address).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Description).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Item).WithOne(p => p.Place)
                .HasForeignKey<Place>(d => d.ItemId)
                .HasConstraintName("FK_Places_Items");
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Plans__755C22D7333F80B8");

            entity.ToTable(tb => tb.HasTrigger("trg_Plans_Update"));

            entity.HasIndex(e => e.IsPublic, "IX_Plans_IsPublic").HasFilter("([IsPublic]=(1))");

            entity.HasIndex(e => e.OwnerId, "IX_Plans_OwnerID");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Owner).WithMany(p => p.Plans)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Plans_Owner");
        });

        modelBuilder.Entity<PlanList>(entity =>
        {
            entity.HasKey(e => e.PlanListId).HasName("PK__PlanList__105E9841BC99124F");

            entity.ToTable("PlanList");

            entity.Property(e => e.PlanListId).HasColumnName("PlanListID");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Notes).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.PlanLists)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlanList_Items");

            entity.HasOne(d => d.Plan).WithMany(p => p.PlanLists)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_PlanList_Plans");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Ratings__FCCDF85C97ACC67D");

            entity.HasIndex(e => new { e.ReferenceType, e.ReferenceId }, "IX_Ratings_Reference").HasFilter("([ReferenceType] IS NOT NULL)");

            entity.Property(e => e.RatingId).HasColumnName("RatingID");
            entity.Property(e => e.Comment).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.ReferenceType).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Item).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK_Ratings_Items");

            entity.HasOne(d => d.Plan).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_Ratings_Plans");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Ratings_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A580E7B37");

            entity.ToTable(tb => tb.HasTrigger("trg_Roles_Update"));

            entity.HasIndex(e => e.Name, "UK_Roles_Name").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<Transportation>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Transpor__727E83EBC55DE59D");

            entity.Property(e => e.ItemId)
                .ValueGeneratedNever()
                .HasColumnName("ItemID");
            entity.Property(e => e.Address).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");

            entity.HasOne(d => d.Item).WithOne(p => p.Transportation)
                .HasForeignKey<Transportation>(d => d.ItemId)
                .HasConstraintName("FK_Transportations_Items");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC480058B6");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("tr_Users_UpdatedAt");
                    tb.HasTrigger("trg_Users_Update");
                });

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.IsActive, "IX_Users_IsActive").HasFilter("([IsActive]=(1))");

            entity.HasIndex(e => e.Email, "UK_Users_Email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.GoogleId)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.PasswordResetToken)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<UserActivationToken>(entity =>
        {
            entity.HasIndex(e => e.Token, "IX_UserActivationTokens_Token");

            entity.HasIndex(e => e.UserId, "IX_UserActivationTokens_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Token).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.UserActivationTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserActivationTokens_Users");
        });

        modelBuilder.Entity<UserChallengeProgress>(entity =>
        {
            entity.HasKey(e => e.UserChallengeProgressId).HasName("PK__UserChal__8703337E9F2A3F2D");

            entity.ToTable("UserChallengeProgress");

            entity.Property(e => e.UserChallengeProgressId).HasColumnName("UserChallengeProgressID");
            entity.Property(e => e.ChallengeId).HasColumnName("ChallengeID");
            entity.Property(e => e.ProofImageId).HasColumnName("ProofImageID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VerificationNotes).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");

            entity.HasOne(d => d.Challenge).WithMany(p => p.UserChallengeProgresses)
                .HasForeignKey(d => d.ChallengeId)
                .HasConstraintName("FK_UserChallengeProgress_Challenges");

            entity.HasOne(d => d.ProofImage).WithMany(p => p.UserChallengeProgresses)
                .HasForeignKey(d => d.ProofImageId)
                .HasConstraintName("FK_UserChallengeProgress_Images");

            entity.HasOne(d => d.User).WithMany(p => p.UserChallengeProgresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserChallengeProgress_Users");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoles_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
