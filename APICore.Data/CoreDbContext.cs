using APICore.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APICore.Data
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Setting> Setting { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<UserToken> UserToken { get; set; }
        public DbSet<BlockedUsers> BlockedUsers { get; set; }
        public DbSet<ReportedUsers> ReportedUsers { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentDate = DateTime.Now;

            var currentChanges = ChangeTracker.Entries<BaseEntity>();
            var currentChangedList = currentChanges.ToList();

            foreach (var entry in currentChangedList)
            {
                var entity = entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = currentDate;
                        entry.Entity.ModifiedAt = currentDate;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedAt = currentDate;
                        entry.Entity.CreatedAt = entry.OriginalValues.GetValue<DateTime>("CreatedAt");
                        break;

                    case EntityState.Detached:
                        break;

                    case EntityState.Deleted:
                        break;

                    case EntityState.Unchanged:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlockedUsers>()
                            .HasOne(b => b.BlockerUser)
                            .WithMany(u => u.Blockers)
                            .HasForeignKey(b => b.BlockerUserId)
                            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BlockedUsers>()
                            .HasOne(b => b.BlockedUser)
                            .WithMany(u => u.Blockeds)
                            .HasForeignKey(b => b.BlockedUserId);

            modelBuilder.Entity<ReportedUsers>()
                            .HasOne(b => b.ReporterUser)
                            .WithMany(u => u.Reporters)
                            .HasForeignKey(b => b.ReporterUserId)
                            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReportedUsers>()
                            .HasOne(b => b.ReportedUser)
                            .WithMany(u => u.Reporteds)
                            .HasForeignKey(b => b.ReportedUserId);

        }
    }
}