namespace MyPathfinderCampaignTracker.Application.Models;

public record ChatMessageDto
{
    public Guid Id { get; init; }
    public Guid CampaignId { get; init; }
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
}

public record ChatMessageRequest(string Content);
