using Claims_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Claims_System.Areas.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<LecturerClaim> LecturerClaims { get; set; }
        public DbSet<LecturerProfile> LecturerProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1-to-1 ApplicationUser - LecturerProfile
            builder.Entity<LecturerProfile>()
                .HasKey(lp => lp.UserId);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.LecturerProfile)
                .WithOne(lp => lp.User)
                .HasForeignKey<LecturerProfile>(lp => lp.UserId);

     
            builder.Entity<LecturerProfile>()
                .Property(p => p.HourlyRate)
                .HasPrecision(18, 2);

            builder.Entity<LecturerClaim>()
                .Property(c => c.HoursWorked)
                .HasPrecision(18, 2);

            builder.Entity<LecturerClaim>()
                .Property(c => c.Rate)
                .HasPrecision(18, 2);
        }
    }
}
