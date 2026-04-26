using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class ActivityLogRepository(AppDbContext context) : IActivityLogRepository
{
    public async Task AddAsync(ActivityLog log)
    {
        context.ActivityLogs.Add(log);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<ActivityLog>> GetRecentByCampaignAsync(Guid campaignId, int count)
        => await context.ActivityLogs
            .Include(l => l.User)
            .Where(l => l.CampaignId == campaignId)
            .OrderByDescending(l => l.OccurredAt)
            .Take(count)
            .ToListAsync();
}
