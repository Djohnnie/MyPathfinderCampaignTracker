namespace MyPathfinderCampaignTracker.Domain.Entities;

public class Recap
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public int Number { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Contents { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
