using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ICharacterService
{
    Task<IReadOnlyList<CharacterDto>> GetByCampaignAsync(Guid campaignId);
    Task<CharacterDto?> GetByIdAsync(Guid id);
    Task<CharacterDto> CreateAsync(Guid campaignId, Guid userId, CharacterRequest request);
    Task<bool> UpdateAsync(Guid id, CharacterRequest request);
    Task<bool> DeleteAsync(Guid id);
}
