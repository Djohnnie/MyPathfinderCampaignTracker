using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class CharacterSheetRepository(AppDbContext context) : ICharacterSheetRepository
{
    public async Task<CharacterSheet?> GetByCharacterIdAsync(Guid characterId)
        => await context.CharacterSheets
            .FirstOrDefaultAsync(s => s.CharacterId == characterId);

    public async Task UpsertAsync(CharacterSheet sheet)
    {
        var existing = await context.CharacterSheets
            .FirstOrDefaultAsync(s => s.CharacterId == sheet.CharacterId);

        if (existing is null)
        {
            context.CharacterSheets.Add(sheet);
        }
        else
        {
            existing.MaxHitPoints = sheet.MaxHitPoints;
            existing.CurrentHitPoints = sheet.CurrentHitPoints;
            existing.TemporaryHitPoints = sheet.TemporaryHitPoints;
            existing.ArmorClass = sheet.ArmorClass;
            existing.TouchArmorClass = sheet.TouchArmorClass;
            existing.FlatFootedArmorClass = sheet.FlatFootedArmorClass;
            existing.InitiativeBonus = sheet.InitiativeBonus;
            existing.Speed = sheet.Speed;
            existing.BaseAttackBonus = sheet.BaseAttackBonus;
            existing.CombatManeuverBonus = sheet.CombatManeuverBonus;
            existing.CombatManeuverDefense = sheet.CombatManeuverDefense;
            existing.FortitudeSave = sheet.FortitudeSave;
            existing.ReflexSave = sheet.ReflexSave;
            existing.WillSave = sheet.WillSave;
            existing.SkillsJson = sheet.SkillsJson;
            existing.Feats = sheet.Feats;
            existing.SpecialAbilities = sheet.SpecialAbilities;
            existing.Equipment = sheet.Equipment;
            existing.Spells = sheet.Spells;
            existing.Notes = sheet.Notes;
            existing.UpdatedAt = sheet.UpdatedAt;
        }

        await context.SaveChangesAsync();
    }
}
