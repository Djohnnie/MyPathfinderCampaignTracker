using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Description).IsRequired();
            entity.Property(c => c.ExtensiveInformation).IsRequired();

            entity.HasMany(c => c.Players)
                  .WithMany()
                  .UsingEntity("CampaignUsers");
        });
    }
}
