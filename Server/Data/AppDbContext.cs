using Microsoft.EntityFrameworkCore;
using Server.Data.Models;

namespace Server;

public class AppDbContext : DbContext
{
    public DbSet<Announcement> Announcements { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=announcementdb;Username=postgres;Password=myPa55word;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>()
            .HasIndex(u => u.Title)
            .IsUnique();

        modelBuilder.Entity<Announcement>()
            .HasData(
                new { Id = 1, Title = "Shawshank Redemption" },
                new { Id = 2, Title = "The Dark Knight" },
                new { Id = 3, Title = "A Bronx Tale" }
            );
    }
}
