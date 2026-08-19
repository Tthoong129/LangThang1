using Microsoft.EntityFrameworkCore;
using MiniMap.Models;

namespace MiniMap.Data
{
    public class TravelReviewDbContext : DbContext
    {
        public TravelReviewDbContext(DbContextOptions<TravelReviewDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Region> Regions => Set<Region>();
        public DbSet<Province> Provinces => Set<Province>();
        public DbSet<PlaceType> PlaceTypes => Set<PlaceType>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<AdminCategoryAssignment> AdminCategoryAssignments => Set<AdminCategoryAssignment>();
        public DbSet<Place> Places => Set<Place>();
        public DbSet<PlaceMedia> PlaceMedia => Set<PlaceMedia>();
        public DbSet<PlaceEditProposal> PlaceEditProposals => Set<PlaceEditProposal>();
        public DbSet<Food> Foods => Set<Food>();
        public DbSet<FoodProvince> FoodProvinces => Set<FoodProvince>();
        public DbSet<FoodPlace> FoodPlaces => Set<FoodPlace>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<ReviewMedia> ReviewMedia => Set<ReviewMedia>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<VisitLog> VisitLogs => Set<VisitLog>();
        public DbSet<AccessHistory> AccessHistories => Set<AccessHistory>();
        public DbSet<ReportReason> ReportReasons => Set<ReportReason>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<Appeal> Appeals => Set<Appeal>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.GoogleId).IsUnique().HasFilter("[GoogleId] IS NOT NULL");
            });

            // 2. Regions & Provinces
            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasIndex(r => r.Name).IsUnique();
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.HasIndex(p => new { p.Name, p.RegionId }).IsUnique();
                entity.HasOne(p => p.Region)
                    .WithMany(r => r.Provinces)
                    .HasForeignKey(p => p.RegionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. PlaceTypes & Categories
            modelBuilder.Entity<PlaceType>(entity =>
            {
                entity.HasIndex(pt => pt.Name).IsUnique();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => new { c.Name, c.PlaceTypeId }).IsUnique();
                entity.HasOne(c => c.PlaceType)
                    .WithMany(pt => pt.Categories)
                    .HasForeignKey(c => c.PlaceTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 4. AdminCategoryAssignments
            modelBuilder.Entity<AdminCategoryAssignment>(entity =>
            {
                entity.HasIndex(a => new { a.UserId, a.CategoryId }).IsUnique();
                entity.HasOne(a => a.User)
                    .WithMany(u => u.CategoryAssignments)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.Category)
                    .WithMany(c => c.AdminAssignments)
                    .HasForeignKey(a => a.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 5. Places
            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasIndex(p => p.ProvinceId);
                entity.HasIndex(p => p.CategoryId);
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => new { p.Latitude, p.Longitude });

                entity.HasOne(p => p.Province)
                    .WithMany(pr => pr.Places)
                    .HasForeignKey(p => p.ProvinceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Places)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Proposer)
                    .WithMany()
                    .HasForeignKey(p => p.ProposedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(p => p.Approver)
                    .WithMany()
                    .HasForeignKey(p => p.ApprovedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // 6. PlaceMedia
            modelBuilder.Entity<PlaceMedia>(entity =>
            {
                entity.HasOne(pm => pm.Place)
                    .WithMany(p => p.Media)
                    .HasForeignKey(pm => pm.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pm => pm.Uploader)
                    .WithMany()
                    .HasForeignKey(pm => pm.UploadedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // 7. PlaceEditProposal
            modelBuilder.Entity<PlaceEditProposal>(entity =>
            {
                entity.HasOne(pep => pep.Place)
                    .WithMany()
                    .HasForeignKey(pep => pep.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pep => pep.Proposer)
                    .WithMany()
                    .HasForeignKey(pep => pep.ProposedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pep => pep.Reviewer)
                    .WithMany()
                    .HasForeignKey(pep => pep.ReviewedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // 8. Foods & Food relations
            modelBuilder.Entity<Food>(entity =>
            {
                entity.HasIndex(f => f.Name).IsUnique();
            });

            modelBuilder.Entity<FoodProvince>(entity =>
            {
                entity.HasKey(fp => new { fp.FoodId, fp.ProvinceId });
                entity.HasOne(fp => fp.Food)
                    .WithMany(f => f.FoodProvinces)
                    .HasForeignKey(fp => fp.FoodId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(fp => fp.Province)
                    .WithMany(p => p.FoodProvinces)
                    .HasForeignKey(fp => fp.ProvinceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FoodPlace>(entity =>
            {
                entity.HasKey(fp => new { fp.FoodId, fp.PlaceId });
                entity.HasOne(fp => fp.Food)
                    .WithMany(f => f.FoodPlaces)
                    .HasForeignKey(fp => fp.FoodId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(fp => fp.Place)
                    .WithMany(p => p.FoodPlaces)
                    .HasForeignKey(fp => fp.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 9. Reviews & Media & Comments
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(r => new { r.PlaceId, r.UserId }).IsUnique();
                entity.HasIndex(r => r.PlaceId);
                entity.HasIndex(r => r.UserId);

                entity.HasOne(r => r.Place)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(r => r.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReviewMedia>(entity =>
            {
                entity.HasOne(rm => rm.Review)
                    .WithMany(r => r.Media)
                    .HasForeignKey(rm => rm.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.Review)
                    .WithMany(r => r.Comments)
                    .HasForeignKey(c => c.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 10. Favorites
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(f => new { f.UserId, f.PlaceId });
                entity.HasOne(f => f.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(f => f.Place)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(f => f.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 11. VisitLogs
            modelBuilder.Entity<VisitLog>(entity =>
            {
                entity.HasOne(v => v.User)
                    .WithMany(u => u.VisitLogs)
                    .HasForeignKey(v => v.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(v => v.Place)
                    .WithMany(p => p.VisitLogs)
                    .HasForeignKey(v => v.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 12. AccessHistory
            modelBuilder.Entity<AccessHistory>(entity =>
            {
                entity.HasIndex(a => new { a.UserId, a.ViewedAt });
                entity.HasOne(a => a.User)
                    .WithMany(u => u.AccessHistories)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.Place)
                    .WithMany(p => p.AccessHistories)
                    .HasForeignKey(a => a.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 13. Reports
            modelBuilder.Entity<ReportReason>(entity =>
            {
                entity.HasIndex(rr => rr.Content).IsUnique();
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasIndex(r => new { r.ReporterId, r.TargetType, r.TargetId }).IsUnique();
                entity.HasOne(r => r.Reporter)
                    .WithMany()
                    .HasForeignKey(r => r.ReporterId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.Reason)
                    .WithMany()
                    .HasForeignKey(r => r.ReasonId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.Handler)
                    .WithMany()
                    .HasForeignKey(r => r.HandledBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // 14. Appeals
            modelBuilder.Entity<Appeal>(entity =>
            {
                entity.HasOne(a => a.User)
                    .WithMany()
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.CategoryAdmin)
                    .WithMany()
                    .HasForeignKey(a => a.CategoryAdminId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(a => a.SystemAdmin)
                    .WithMany()
                    .HasForeignKey(a => a.SystemAdminId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // 15. Notifications
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(n => new { n.UserId, n.IsRead });
                entity.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 16. SystemConfigs
            modelBuilder.Entity<SystemConfig>(entity =>
            {
                entity.HasIndex(sc => sc.ConfigKey).IsUnique();
                entity.HasOne(sc => sc.Updater)
                    .WithMany()
                    .HasForeignKey(sc => sc.UpdatedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // 17. AuditLogs
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasIndex(al => new { al.UserId, al.CreatedAt });
                entity.HasOne(al => al.User)
                    .WithMany()
                    .HasForeignKey(al => al.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
