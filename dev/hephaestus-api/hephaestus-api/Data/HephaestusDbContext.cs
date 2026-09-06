using Hephaestus.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Api.Data;

public sealed class HephaestusDbContext(DbContextOptions<HephaestusDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();
    public DbSet<TwoFactorChallenge> TwoFactorChallenges => Set<TwoFactorChallenge>();
    public DbSet<ExternalLoginCode> ExternalLoginCodes => Set<ExternalLoginCode>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketStatus> TicketStatuses => Set<TicketStatus>();
    public DbSet<Priority> Priorities => Set<Priority>();
    public DbSet<WorkTask> Tasks => Set<WorkTask>();
    public DbSet<WorkTaskStatus> TaskStatuses => Set<WorkTaskStatus>();
    public DbSet<AttachmentAndHistory> AttachmentsAndHistory => Set<AttachmentAndHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.UsersId);
            entity.Property(x => x.UsersId).HasColumnName("UsersID");
            entity.Property(x => x.RolesId).HasColumnName("RolesID");
            entity.Property(x => x.LocationsId).HasColumnName("LocationsID");
            entity.Property(x => x.Email).HasMaxLength(150);
            entity.Property(x => x.PasswordHash).HasMaxLength(255);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RolesId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(x => x.RolesId);
            entity.Property(x => x.RolesId).HasColumnName("RolesID");
            entity.Property(x => x.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<AuthSession>(entity =>
        {
            entity.ToTable("AuthSessions");
            entity.HasKey(x => x.AuthSessionId);
            entity.Property(x => x.AuthSessionId).HasColumnName("AuthSessionsID");
            entity.Property(x => x.UsersId).HasColumnName("UsersID");
            entity.Property(x => x.RefreshTokenHash).HasMaxLength(64).IsUnicode(false);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime2(0)");
            entity.Property(x => x.ExpiresAt).HasColumnType("datetime2(0)");
            entity.Property(x => x.RevokedAt).HasColumnType("datetime2(0)");
            entity.HasIndex(x => x.RefreshTokenHash).IsUnique();
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UsersId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TwoFactorChallenge>(entity =>
        {
            entity.ToTable("TwoFactorChallenges");
            entity.HasKey(x => x.TwoFactorChallengeId);
            entity.Property(x => x.TwoFactorChallengeId).HasColumnName("TwoFactorChallengesID");
            entity.Property(x => x.UsersId).HasColumnName("UsersID");
            entity.Property(x => x.CodeHash).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime2(0)");
            entity.Property(x => x.ExpiresAt).HasColumnType("datetime2(0)");
            entity.Property(x => x.UsedAt).HasColumnType("datetime2(0)");
            entity.Property(x => x.FailedAttempts).HasDefaultValue(0);
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UsersId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ExternalLoginCode>(entity =>
        {
            entity.ToTable("ExternalLoginCodes");
            entity.HasKey(x => x.ExternalLoginCodeId);
            entity.Property(x => x.ExternalLoginCodeId).HasColumnName("ExternalLoginCodesID");
            entity.Property(x => x.UsersId).HasColumnName("UsersID");
            entity.Property(x => x.CodeHash).HasMaxLength(64).IsUnicode(false);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime2(0)");
            entity.Property(x => x.ExpiresAt).HasColumnType("datetime2(0)");
            entity.Property(x => x.UsedAt).HasColumnType("datetime2(0)");
            entity.HasIndex(x => x.CodeHash).IsUnique();
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UsersId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.ToTable("Priorities");
            entity.HasKey(x => x.PrioritiesId);
            entity.Property(x => x.PrioritiesId).HasColumnName("PrioritiesID");
            entity.Property(x => x.Code).HasMaxLength(10).IsUnicode(false);
            entity.Property(x => x.Description).HasMaxLength(100).IsUnicode(false);
            entity.Property(x => x.TicketType).HasMaxLength(10).IsUnicode(false);
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.ToTable("TicketStatuses");
            entity.HasKey(x => x.TicketStatusesId);
            entity.Property(x => x.TicketStatusesId).HasColumnName("TicketStatusesID");
            entity.Property(x => x.Name).HasMaxLength(50).IsUnicode(false);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Tickets");
            entity.HasKey(x => x.TicketsId);
            entity.Property(x => x.TicketsId).HasColumnName("TicketsID");
            entity.Property(x => x.PrioritiesId).HasColumnName("PrioritiesID");
            entity.Property(x => x.TicketStatusesId).HasColumnName("TicketStatusesID");
            entity.Property(x => x.CreatedById).HasColumnName("CreatedByID");
            entity.Property(x => x.AssignedToId).HasColumnName("AssignedToID");
            entity.Property(x => x.ReferenceCode).HasMaxLength(20).IsUnicode(false);
            entity.Property(x => x.TicketType).HasMaxLength(10).IsUnicode(false);
            entity.Property(x => x.Title).HasMaxLength(200).IsUnicode(false);
            entity.Property(x => x.Description).HasColumnType("text");
            entity.Property(x => x.OpenDate).HasColumnType("datetime");
            entity.HasOne(x => x.Priority).WithMany().HasForeignKey(x => x.PrioritiesId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.TicketStatusesId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.AssignedTo).WithMany().HasForeignKey(x => x.AssignedToId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<WorkTaskStatus>(entity =>
        {
            entity.ToTable("TaskStatuses");
            entity.HasKey(x => x.TaskStatusesId);
            entity.Property(x => x.TaskStatusesId).HasColumnName("TaskStatusesID");
            entity.Property(x => x.Name).HasMaxLength(50).IsUnicode(false);
        });

        modelBuilder.Entity<WorkTask>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(x => x.TasksId);
            entity.Property(x => x.TasksId).HasColumnName("TasksID");
            entity.Property(x => x.TicketsId).HasColumnName("TicketsID");
            entity.Property(x => x.TechnicianId).HasColumnName("TechnicianID");
            entity.Property(x => x.TaskStatusesId).HasColumnName("TaskStatusesID");
            entity.Property(x => x.ReferenceCode).HasMaxLength(20).IsUnicode(false);
            entity.Property(x => x.ScheduledStart).HasColumnType("datetime");
            entity.Property(x => x.ScheduledEnd).HasColumnType("datetime");
            entity.Property(x => x.ActualStartDate).HasColumnType("datetime");
            entity.Property(x => x.ActualEndDate).HasColumnType("datetime");
            entity.HasOne(x => x.Ticket).WithMany().HasForeignKey(x => x.TicketsId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Technician).WithMany().HasForeignKey(x => x.TechnicianId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.TaskStatusesId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AttachmentAndHistory>(entity =>
        {
            entity.ToTable("AttachmentsAndHistory");
            entity.HasKey(x => x.AttachmentsAndHistoryId);
            entity.Property(x => x.AttachmentsAndHistoryId).HasColumnName("AttachmentsAndHistoryID");
            entity.Property(x => x.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(x => x.UsersId).HasColumnName("UsersID");
            entity.Property(x => x.EntityType).HasMaxLength(10).IsUnicode(false);
            entity.Property(x => x.RecordType).HasMaxLength(20).IsUnicode(false);
            entity.Property(x => x.Content).HasColumnType("text");
            entity.Property(x => x.CreatedDate).HasColumnType("datetime");
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UsersId).OnDelete(DeleteBehavior.NoAction);
        });

    }
}
