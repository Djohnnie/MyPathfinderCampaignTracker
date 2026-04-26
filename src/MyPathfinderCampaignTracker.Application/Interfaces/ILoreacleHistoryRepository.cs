using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ILoreacleHistoryRepository
{
    Task<IReadOnlyList<LoreacleMessage>> GetByCampaignAsync(Guid campaignId);
    Task AddAsync(LoreacleMessage message);
    Task MarkAsCompactedAsync(IEnumerable<Guid> ids);
}
