using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ICampaignNoteService
{
    Task<IReadOnlyList<CampaignNoteDto>> GetByCampaignAsync(Guid campaignId);
    Task<CampaignNoteDto?> GetByIdAsync(Guid id);
    Task<CampaignNoteDto> CreateAsync(Guid campaignId, Guid userId, CampaignNoteRequest request);
    Task<bool> UpdateAsync(Guid id, CampaignNoteRequest request);
    Task<bool> DeleteAsync(Guid id);
}
