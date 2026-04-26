namespace MyPathfinderCampaignTracker.Application.Models;

public class GameSessionDto
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public record GameSessionRequest(DateTime ScheduledAt, string Location);
