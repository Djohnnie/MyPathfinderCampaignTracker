using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ICharacterSheetRepository
{
    Task<CharacterSheet?> GetByCharacterIdAsync(Guid characterId);
    Task UpsertAsync(CharacterSheet sheet);
}
