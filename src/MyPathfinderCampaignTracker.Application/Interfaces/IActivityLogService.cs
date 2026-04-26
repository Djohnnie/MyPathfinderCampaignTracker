using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(Guid campaignId, Guid userId, ActivityType activityType, string? subjectName = null);
    Task<IReadOnlyList<ActivityLogDto>> GetRecentAsync(Guid campaignId, int count = 10);
}
