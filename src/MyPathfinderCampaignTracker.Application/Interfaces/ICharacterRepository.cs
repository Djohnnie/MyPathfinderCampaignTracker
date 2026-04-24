using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ICharacterRepository
{
    Task<IReadOnlyList<Character>> GetByCampaignAsync(Guid campaignId);
    Task<Character?> GetByIdAsync(Guid id);
    Task AddAsync(Character character);
    Task UpdateAsync(Character character);
    Task DeleteAsync(Guid id);
}
