namespace MyPathfinderCampaignTracker.Domain.Entities;

public class Character
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string CharacterClass { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public string Backstory { get; set; } = string.Empty;
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    public bool KilledInAction { get; set; }
    public string? Alignment { get; set; }
    public string? Personality { get; set; }
    public string? IdealsAndGoals { get; set; }
    public string? Flaws { get; set; }
    public string? Languages { get; set; }
    public string? Appearance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
