using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ICampaignNoteRepository
{
    Task<IReadOnlyList<CampaignNote>> GetByCampaignAsync(Guid campaignId);
    Task<CampaignNote?> GetByIdAsync(Guid id);
    Task AddAsync(CampaignNote note);
    Task UpdateAsync(CampaignNote note);
    Task DeleteAsync(Guid id);
}
