namespace MyPathfinderCampaignTracker.Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }

    public Campaign Campaign { get; set; } = default!;
    public User User { get; set; } = default!;
}
