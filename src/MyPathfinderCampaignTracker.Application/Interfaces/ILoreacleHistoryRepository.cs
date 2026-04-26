using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ILoreacleHistoryRepository
{
    Task<IReadOnlyList<LoreacleMessage>> GetByCampaignAndUserAsync(Guid campaignId, Guid userId);
    Task AddAsync(LoreacleMessage message);
    Task MarkAsCompactedAsync(IEnumerable<Guid> ids);
}
