using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PlanyApp.Repository.Models;

public partial class PlanyDbContext : DbContext
{
    public PlanyDbContext()
    {
    }

    public PlanyDbContext(DbContextOptions<PlanyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=plany-db.japao.dev,11433;Database=PlanyDB;User Id=sa;Password=Pl4nyDBMSSQL!!;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B1C833A78");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
        });

        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.ChallengeId).HasName("PK__Challeng__C7AC8128F8DEDA31");

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
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__149AF30A558FF80E");

            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.Description).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
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
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Items__727E83EB9B2DA26E");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ItemType)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
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
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Plans__755C22D7333F80B8");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18, 2)");
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
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Ratings__FCCDF85C97ACC67D");

            entity.Property(e => e.RatingId).HasColumnName("RatingID");
            entity.Property(e => e.Comment).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A580E7B37");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
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
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC480058B6");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
            entity.Property(e => e.GoogleId)
                .HasMaxLength(255)
                .UseCollation("Latin1_General_100_CI_AS_SC_UTF8");
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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
