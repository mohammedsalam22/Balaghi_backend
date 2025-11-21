using Microsoft.EntityFrameworkCore;
using Domain.Entities;
namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<OtpCode> OtpCodes { get; set; }
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        public DbSet<GovernmentAgency> GovernmentAgencies => Set<GovernmentAgency>();
        public DbSet<PasswordSetupOtp> PasswordSetupOtps => Set<PasswordSetupOtp>();
        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<ComplaintAttachment> ComplaintAttachments => Set<ComplaintAttachment>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });
            modelBuilder.Entity<OtpCode>()
                .HasKey(o => o.Id);
            modelBuilder.Entity<User>()
                .HasOne(u => u.GovernmentAgency)
                .WithMany(a => a.Employees)
                .HasForeignKey(u => u.GovernmentAgencyId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<PasswordSetupOtp>()
                .HasOne(p => p.User)
                .WithMany(u => u.PasswordSetupOtps)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.TrackingNumber)
                      .IsRequired()
                      .HasMaxLength(20);
                entity.HasIndex(c => c.TrackingNumber)
                      .IsUnique();
                entity.Property(c => c.ComplaintType)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(c => c.Description)
                      .IsRequired();
                entity.Property(c => c.Status)
                      .HasDefaultValue(ComplaintStatus.Pending);
                entity.HasOne(c => c.Citizen)
                      .WithMany(u => u.Complaints)
                      .HasForeignKey(c => c.CitizenId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.Agency)
                      .WithMany(a => a.Complaints)
                      .HasForeignKey(c => c.AgencyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ComplaintAttachment>(entity =>
            {
                entity.HasKey(ca => ca.Id);

                entity.Property(ca => ca.FileName)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(ca => ca.FilePath)
                      .IsRequired();

                entity.Property(ca => ca.ContentType)
                      .HasMaxLength(100);

                entity.HasOne(ca => ca.Complaint)
                      .WithMany(c => c.Attachments)
                      .HasForeignKey(ca => ca.ComplaintId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
