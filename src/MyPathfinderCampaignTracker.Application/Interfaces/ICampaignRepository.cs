using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ICampaignRepository
{
    Task<IReadOnlyList<Campaign>> GetAllAsync();
    Task<Campaign?> GetByIdAsync(Guid id);
    Task AddAsync(Campaign campaign);
    Task UpdateAsync(Campaign campaign);
    Task DeleteAsync(Guid id);
    Task AddPlayerAsync(Guid campaignId, Guid userId);
    Task RemovePlayerAsync(Guid campaignId, Guid userId);
}
