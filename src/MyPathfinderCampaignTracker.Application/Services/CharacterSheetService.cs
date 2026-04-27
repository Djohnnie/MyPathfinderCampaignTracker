using System.Text.Json;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Services;

public class CharacterSheetService(ICharacterSheetRepository sheetRepository) : ICharacterSheetService
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<CharacterSheetDto> GetOrCreateAsync(Guid characterId)
    {
        var sheet = await sheetRepository.GetByCharacterIdAsync(characterId);

        if (sheet is null)
        {
            sheet = new CharacterSheet
            {
                Id = Guid.NewGuid(),
                CharacterId = characterId,
                ArmorClass = 10,
                TouchArmorClass = 10,
                FlatFootedArmorClass = 10,
                Speed = 30,
                CombatManeuverDefense = 10,
                SkillsJson = JsonSerializer.Serialize(DefaultSkills(), _jsonOptions),
                UpdatedAt = DateTime.UtcNow
            };
            await sheetRepository.UpsertAsync(sheet);
        }

        return MapToDto(sheet);
    }

    public async Task<bool> UpdateAsync(Guid characterId, CharacterSheetRequest request)
    {
        var existing = await sheetRepository.GetByCharacterIdAsync(characterId);

        var sheet = existing ?? new CharacterSheet { Id = Guid.NewGuid(), CharacterId = characterId };

        sheet.MaxHitPoints = request.MaxHitPoints;
        sheet.CurrentHitPoints = request.CurrentHitPoints;
        sheet.TemporaryHitPoints = request.TemporaryHitPoints;
        sheet.ArmorClass = request.ArmorClass;
        sheet.TouchArmorClass = request.TouchArmorClass;
        sheet.FlatFootedArmorClass = request.FlatFootedArmorClass;
        sheet.InitiativeBonus = request.InitiativeBonus;
        sheet.Speed = request.Speed;
        sheet.BaseAttackBonus = request.BaseAttackBonus;
        sheet.CombatManeuverBonus = request.CombatManeuverBonus;
        sheet.CombatManeuverDefense = request.CombatManeuverDefense;
        sheet.FortitudeSave = request.FortitudeSave;
        sheet.ReflexSave = request.ReflexSave;
        sheet.WillSave = request.WillSave;
        sheet.SkillsJson = JsonSerializer.Serialize(request.Skills, _jsonOptions);
        sheet.Feats = request.Feats;
        sheet.SpecialAbilities = request.SpecialAbilities;
        sheet.Equipment = request.Equipment;
        sheet.Spells = request.Spells;
        sheet.Notes = request.Notes;
        sheet.UpdatedAt = DateTime.UtcNow;

        await sheetRepository.UpsertAsync(sheet);
        return true;
    }

    private static CharacterSheetDto MapToDto(CharacterSheet sheet)
    {
        List<SkillEntryDto> skills = [];

        if (!string.IsNullOrWhiteSpace(sheet.SkillsJson))
        {
            try
            {
                skills = JsonSerializer.Deserialize<List<SkillEntryDto>>(sheet.SkillsJson, _jsonOptions) ?? [];
            }
            catch { skills = []; }
        }

        if (skills.Count == 0)
            skills = DefaultSkillDtos();

        return new CharacterSheetDto
        {
            Id = sheet.Id,
            CharacterId = sheet.CharacterId,
            MaxHitPoints = sheet.MaxHitPoints,
            CurrentHitPoints = sheet.CurrentHitPoints,
            TemporaryHitPoints = sheet.TemporaryHitPoints,
            ArmorClass = sheet.ArmorClass,
            TouchArmorClass = sheet.TouchArmorClass,
            FlatFootedArmorClass = sheet.FlatFootedArmorClass,
            InitiativeBonus = sheet.InitiativeBonus,
            Speed = sheet.Speed,
            BaseAttackBonus = sheet.BaseAttackBonus,
            CombatManeuverBonus = sheet.CombatManeuverBonus,
            CombatManeuverDefense = sheet.CombatManeuverDefense,
            FortitudeSave = sheet.FortitudeSave,
            ReflexSave = sheet.ReflexSave,
            WillSave = sheet.WillSave,
            Skills = skills,
            Feats = sheet.Feats,
            SpecialAbilities = sheet.SpecialAbilities,
            Equipment = sheet.Equipment,
            Spells = sheet.Spells,
            Notes = sheet.Notes,
            UpdatedAt = sheet.UpdatedAt
        };
    }

    // Default 35 Pathfinder skills serialized for storage
    private static List<SkillEntryRequest> DefaultSkills() =>
        DefaultSkillDtos().Select(s => new SkillEntryRequest(s.Name, s.AbilityKey, s.IsClassSkill, s.Ranks, s.MiscBonus, s.TrainedOnly)).ToList();

    internal static List<SkillEntryDto> DefaultSkillDtos() =>
    [
        new() { Name = "Acrobatics",            AbilityKey = "DEX", TrainedOnly = false },
        new() { Name = "Appraise",              AbilityKey = "INT", TrainedOnly = false },
        new() { Name = "Bluff",                 AbilityKey = "CHA", TrainedOnly = false },
        new() { Name = "Climb",                 AbilityKey = "STR", TrainedOnly = false },
        new() { Name = "Craft",                 AbilityKey = "INT", TrainedOnly = false },
        new() { Name = "Diplomacy",             AbilityKey = "CHA", TrainedOnly = false },
        new() { Name = "Disable Device",        AbilityKey = "DEX", TrainedOnly = true  },
        new() { Name = "Disguise",              AbilityKey = "CHA", TrainedOnly = false },
        new() { Name = "Escape Artist",         AbilityKey = "DEX", TrainedOnly = false },
        new() { Name = "Fly",                   AbilityKey = "DEX", TrainedOnly = false },
        new() { Name = "Handle Animal",         AbilityKey = "CHA", TrainedOnly = true  },
        new() { Name = "Heal",                  AbilityKey = "WIS", TrainedOnly = false },
        new() { Name = "Intimidate",            AbilityKey = "CHA", TrainedOnly = false },
        new() { Name = "Knowledge (Arcana)",    AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Knowledge (Dungeoneering)", AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Knowledge (Engineering)", AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Knowledge (Geography)", AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Knowledge (History)",   AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Knowledge (Local)",     AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Knowledge (Nature)",    AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Knowledge (Nobility)",  AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Knowledge (Planes)",    AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Knowledge (Religion)",  AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Linguistics",           AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Perception",            AbilityKey = "WIS", TrainedOnly = false },
        new() { Name = "Perform",               AbilityKey = "CHA", TrainedOnly = false },
        new() { Name = "Profession",            AbilityKey = "WIS", TrainedOnly = true  },
        new() { Name = "Ride",                  AbilityKey = "DEX", TrainedOnly = false },
        new() { Name = "Sense Motive",          AbilityKey = "WIS", TrainedOnly = false },
        new() { Name = "Sleight of Hand",       AbilityKey = "DEX", TrainedOnly = true  },
        new() { Name = "Spellcraft",            AbilityKey = "INT", TrainedOnly = true  },
        new() { Name = "Stealth",               AbilityKey = "DEX", TrainedOnly = false },
        new() { Name = "Survival",              AbilityKey = "WIS", TrainedOnly = false },
        new() { Name = "Swim",                  AbilityKey = "STR", TrainedOnly = false },
        new() { Name = "Use Magic Device",      AbilityKey = "CHA", TrainedOnly = true  },
    ];
}
