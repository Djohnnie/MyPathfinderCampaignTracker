using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ICampaignService
{
    Task<IReadOnlyList<CampaignDto>> GetAllAsync();
    Task<IReadOnlyList<CampaignDto>> GetByPlayerAsync(Guid userId);
    Task<CampaignDto?> GetByIdAsync(Guid id);
    Task<CampaignDto> CreateAsync(CampaignRequest request);
    Task<bool> UpdateAsync(Guid id, CampaignRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> AddPlayerAsync(Guid campaignId, Guid userId);
    Task<bool> RemovePlayerAsync(Guid campaignId, Guid userId);
    Task<bool> UpdateDescriptionAsync(Guid campaignId, string description);
}
