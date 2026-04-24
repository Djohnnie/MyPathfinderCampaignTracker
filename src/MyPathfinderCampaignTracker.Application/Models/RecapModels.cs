namespace MyPathfinderCampaignTracker.Application.Models;

public class RecapDto
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
    public int Number { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Contents { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public record RecapRequest(
    DateTime Date,
    string Title,
    string Contents);
