using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Recap> Recaps => Set<Recap>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();

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

        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Race).IsRequired().HasMaxLength(100);
            entity.Property(c => c.CharacterClass).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Backstory).IsRequired();

            entity.HasOne(c => c.Campaign)
                  .WithMany()
                  .HasForeignKey(c => c.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.User)
                  .WithMany()
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Recap>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Title).IsRequired().HasMaxLength(200);
            entity.Property(r => r.Contents).IsRequired();

            entity.HasOne(r => r.Campaign)
                  .WithMany()
                  .HasForeignKey(r => r.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.User)
                  .WithMany()
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.CampaignId, r.Number }).IsUnique();
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Content).IsRequired();

            entity.HasOne(m => m.Campaign)
                  .WithMany()
                  .HasForeignKey(m => m.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.User)
                  .WithMany()
                  .HasForeignKey(m => m.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(m => new { m.CampaignId, m.SentAt });
        });

        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Location).IsRequired().HasMaxLength(300);

            entity.HasOne(s => s.Campaign)
                  .WithMany()
                  .HasForeignKey(s => s.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => new { s.CampaignId, s.ScheduledAt });
        });
    }
}
