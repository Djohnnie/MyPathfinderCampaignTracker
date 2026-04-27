using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ICharacterSheetService
{
    /// <summary>
    /// Returns the character sheet for the given character,
    /// auto-creating a blank one seeded with all 35 Pathfinder skills if it does not yet exist.
    /// </summary>
    Task<CharacterSheetDto> GetOrCreateAsync(Guid characterId);

    Task<bool> UpdateAsync(Guid characterId, CharacterSheetRequest request);
}
