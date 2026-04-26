namespace MyPathfinderCampaignTracker.Domain.Entities;

public class LoreacleMessage
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public bool IsUser { get; set; }
    public bool IsCompacted { get; set; }
    public bool IsCompaction { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
