using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
    public DbSet<CampaignNote> CampaignNotes => Set<CampaignNote>();
    public DbSet<LoreacleMessage> LoreacleMessages => Set<LoreacleMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.SysId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.HasKey(u => u.Id).HasAnnotation("SqlServer:Clustered", false);
            entity.HasIndex(u => u.SysId).IsUnique().HasAnnotation("SqlServer:Clustered", true);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();

            entity.HasOne<Campaign>()
                  .WithMany()
                  .HasForeignKey(u => u.FavoriteCampaignId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .IsRequired(false);
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.Property(c => c.SysId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.HasKey(c => c.Id).HasAnnotation("SqlServer:Clustered", false);
            entity.HasIndex(c => c.SysId).IsUnique().HasAnnotation("SqlServer:Clustered", true);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Description).IsRequired();
            entity.Property(c => c.ExtensiveInformation).IsRequired();

            entity.HasMany(c => c.Players)
                  .WithMany()
                  .UsingEntity("CampaignUsers");

            modelBuilder.Entity("CampaignUsers", e =>
            {
                e.HasKey("CampaignId", "PlayersId").HasAnnotation("SqlServer:Clustered", false);
                e.Property<int>("SysId").ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                e.HasIndex("SysId").IsUnique().HasAnnotation("SqlServer:Clustered", true);
            });
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.Property(c => c.SysId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.HasKey(c => c.Id).HasAnnotation("SqlServer:Clustered", false);
            entity.HasIndex(c => c.SysId).IsUnique().HasAnnotation("SqlServer:Clustered", true);
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
            entity.Property(r => r.SysId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.HasKey(r => r.Id).HasAnnotation("SqlServer:Clustered", false);
            entity.HasIndex(r => r.SysId).IsUnique().HasAnnotation("SqlServer:Clustered", true);
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
            entity.Property(m => m.SysId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.HasKey(m => m.Id).HasAnnotation("SqlServer:Clustered", false);
            entity.HasIndex(m => m.SysId).IsUnique().HasAnnotation("SqlServer:Clustered", true);
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
            entity.Property(s => s.SysId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.HasKey(s => s.Id).HasAnnotation("SqlServer:Clustered", false);
            entity.HasIndex(s => s.SysId).IsUnique().HasAnnotation("SqlServer:Clustered", true);
            entity.Property(s => s.Location).IsRequired().HasMaxLength(300);

            entity.HasOne(s => s.Campaign)
                  .WithMany()
                  .HasForeignKey(s => s.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => new { s.CampaignId, s.ScheduledAt });
        });

        modelBuilder.Entity<CampaignNote>(entity =>
        {
            entity.Property(n => n.SysId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.HasKey(n => n.Id).HasAnnotation("SqlServer:Clustered", false);
            entity.HasIndex(n => n.SysId).IsUnique().HasAnnotation("SqlServer:Clustered", true);
            entity.Property(n => n.Content).IsRequired();

            entity.HasOne(n => n.Campaign)
                  .WithMany()
                  .HasForeignKey(n => n.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(n => n.User)
                  .WithMany()
                  .HasForeignKey(n => n.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(n => new { n.CampaignId, n.CreatedAt });
        });

        modelBuilder.Entity<LoreacleMessage>(entity =>
        {
            entity.Property(m => m.SysId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.HasKey(m => m.Id).HasAnnotation("SqlServer:Clustered", false);
            entity.HasIndex(m => m.SysId).IsUnique().HasAnnotation("SqlServer:Clustered", true);
            entity.Property(m => m.Content).IsRequired();

            entity.HasOne(m => m.Campaign)
                  .WithMany()
                  .HasForeignKey(m => m.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.User)
                  .WithMany()
                  .HasForeignKey(m => m.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(m => new { m.CampaignId, m.UserId, m.SentAt });
        });
    }
}
