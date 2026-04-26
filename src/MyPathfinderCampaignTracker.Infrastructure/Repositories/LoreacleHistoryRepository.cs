using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class LoreacleHistoryRepository(AppDbContext dbContext) : ILoreacleHistoryRepository
{
    public async Task<IReadOnlyList<LoreacleMessage>> GetByCampaignAndUserAsync(Guid campaignId, Guid userId) =>
        await dbContext.LoreacleMessages
            .Where(m => m.CampaignId == campaignId && m.UserId == userId && !m.IsCleared)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

    public async Task AddAsync(LoreacleMessage message)
    {
        await dbContext.LoreacleMessages.AddAsync(message);
        await dbContext.SaveChangesAsync();
    }

    public async Task MarkAsCompactedAsync(IEnumerable<Guid> ids)
    {
        var idSet = ids.ToHashSet();
        await dbContext.LoreacleMessages
            .Where(m => idSet.Contains(m.Id))
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsCompacted, true));
    }

    public async Task ClearByCampaignAndUserAsync(Guid campaignId, Guid userId) =>
        await dbContext.LoreacleMessages
            .Where(m => m.CampaignId == campaignId && m.UserId == userId && !m.IsCleared)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsCleared, true));
}
