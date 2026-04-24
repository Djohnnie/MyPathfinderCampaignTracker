using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IRecapRepository
{
    Task<IReadOnlyList<Recap>> GetByCampaignAsync(Guid campaignId);
    Task<IReadOnlyList<Recap>> GetByUserAsync(Guid userId);
    Task<Recap?> GetByIdAsync(Guid id);
    Task<int> GetMaxNumberAsync(Guid campaignId);
    Task AddAsync(Recap recap);
    Task UpdateAsync(Recap recap);
    Task DeleteAsync(Guid id);
}
