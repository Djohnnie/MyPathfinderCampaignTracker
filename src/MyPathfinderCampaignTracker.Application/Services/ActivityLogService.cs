using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Services;

public class ActivityLogService(IActivityLogRepository repository) : IActivityLogService
{
    public async Task LogAsync(Guid campaignId, Guid userId, ActivityType activityType, string? subjectName = null)
    {
        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            UserId = userId,
            ActivityType = activityType,
            SubjectName = subjectName,
            OccurredAt = DateTime.UtcNow
        };
        await repository.AddAsync(log);
    }

    public async Task<IReadOnlyList<ActivityLogDto>> GetRecentAsync(Guid campaignId, int count = 10)
    {
        // Fetch a larger pool so deduplication still yields enough results
        var logs = await repository.GetRecentByCampaignAsync(campaignId, count * 10);

        // Keep only the most recent entry per (user, activity type, subject) combination
        return logs
            .GroupBy(l => (l.UserId, l.ActivityType, l.SubjectName))
            .Select(g => g.First()) // already ordered by OccurredAt desc, so First() = most recent
            .OrderByDescending(l => l.OccurredAt)
            .Take(count)
            .Select(l => new ActivityLogDto
            {
                Id = l.Id,
                CampaignId = l.CampaignId,
                UserId = l.UserId,
                Username = l.User.Username,
                ActivityType = l.ActivityType,
                SubjectName = l.SubjectName,
                OccurredAt = l.OccurredAt
            }).ToList();
    }
}
