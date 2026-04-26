using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Models;

public class ActivityLogDto
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public string? SubjectName { get; set; }
    public DateTime OccurredAt { get; set; }
}
