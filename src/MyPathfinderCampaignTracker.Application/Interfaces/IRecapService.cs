using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IRecapService
{
    Task<IReadOnlyList<RecapDto>> GetByCampaignAsync(Guid campaignId);
    Task<RecapDto?> GetByIdAsync(Guid id);
    Task<RecapDto> CreateAsync(Guid campaignId, Guid userId, RecapRequest request);
    Task<bool> UpdateAsync(Guid id, RecapRequest request);
    Task<bool> DeleteAsync(Guid id);
}
