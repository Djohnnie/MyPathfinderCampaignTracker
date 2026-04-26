using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IActivityLogRepository
{
    Task AddAsync(ActivityLog log);
    Task<IReadOnlyList<ActivityLog>> GetRecentByCampaignAsync(Guid campaignId, int count);
}
