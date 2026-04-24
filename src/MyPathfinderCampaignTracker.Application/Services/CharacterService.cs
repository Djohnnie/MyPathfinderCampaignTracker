using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Services;

public class CharacterService(ICharacterRepository characterRepository) : ICharacterService
{
    public async Task<IReadOnlyList<CharacterDto>> GetByCampaignAsync(Guid campaignId)
    {
        var characters = await characterRepository.GetByCampaignAsync(campaignId);
        return characters.Select(c => MapToDto(c)).ToList();
    }

    public async Task<IReadOnlyList<CharacterDto>> GetByUserAsync(Guid userId)
    {
        var characters = await characterRepository.GetByUserAsync(userId);
        return characters.Select(c => MapToDto(c, includeCampaignName: true)).ToList();
    }

    public async Task<CharacterDto?> GetByIdAsync(Guid id)
    {
        var character = await characterRepository.GetByIdAsync(id);
        return character is null ? null : MapToDto(character);
    }

    public async Task<CharacterDto> CreateAsync(Guid campaignId, Guid userId, CharacterRequest request)
    {
        var now = DateTime.UtcNow;
        var character = new Character
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            UserId = userId,
            Name = request.Name,
            Race = request.Race,
            CharacterClass = request.CharacterClass,
            Level = request.Level,
            Backstory = request.Backstory,
            Strength = request.Strength,
            Dexterity = request.Dexterity,
            Constitution = request.Constitution,
            Intelligence = request.Intelligence,
            Wisdom = request.Wisdom,
            Charisma = request.Charisma,
            KilledInAction = request.KilledInAction,
            CreatedAt = now,
            UpdatedAt = now
        };

        await characterRepository.AddAsync(character);
        return MapToDto(character);
    }

    public async Task<bool> UpdateAsync(Guid id, CharacterRequest request)
    {
        var character = await characterRepository.GetByIdAsync(id);
        if (character is null) return false;

        character.Name = request.Name;
        character.Race = request.Race;
        character.CharacterClass = request.CharacterClass;
        character.Level = request.Level;
        character.Backstory = request.Backstory;
        character.Strength = request.Strength;
        character.Dexterity = request.Dexterity;
        character.Constitution = request.Constitution;
        character.Intelligence = request.Intelligence;
        character.Wisdom = request.Wisdom;
        character.Charisma = request.Charisma;
        character.KilledInAction = request.KilledInAction;
        character.UpdatedAt = DateTime.UtcNow;

        await characterRepository.UpdateAsync(character);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var character = await characterRepository.GetByIdAsync(id);
        if (character is null) return false;

        await characterRepository.DeleteAsync(id);
        return true;
    }

    private static CharacterDto MapToDto(Character c, bool includeCampaignName = false) => new()
    {
        Id = c.Id,
        CampaignId = c.CampaignId,
        CampaignName = includeCampaignName ? (c.Campaign?.Title ?? string.Empty) : string.Empty,
        UserId = c.UserId,
        OwnerUsername = c.User?.Username ?? string.Empty,
        Name = c.Name,
        Race = c.Race,
        CharacterClass = c.CharacterClass,
        Level = c.Level,
        Backstory = c.Backstory,
        Strength = c.Strength,
        Dexterity = c.Dexterity,
        Constitution = c.Constitution,
        Intelligence = c.Intelligence,
        Wisdom = c.Wisdom,
        Charisma = c.Charisma,
        KilledInAction = c.KilledInAction,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
