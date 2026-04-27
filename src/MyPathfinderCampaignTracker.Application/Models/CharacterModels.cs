namespace MyPathfinderCampaignTracker.Application.Models;

public class CharacterDto
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string OwnerUsername { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string CharacterClass { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Backstory { get; set; } = string.Empty;
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    public bool KilledInAction { get; set; }
    public string? Alignment { get; set; }
    public string? Personality { get; set; }
    public string? IdealsAndGoals { get; set; }
    public string? Flaws { get; set; }
    public string? Languages { get; set; }
    public string? Appearance { get; set; }
    public bool HasPhoto { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public record CharacterRequest(
    string Name,
    string Race,
    string CharacterClass,
    int Level,
    string Backstory,
    int Strength,
    int Dexterity,
    int Constitution,
    int Intelligence,
    int Wisdom,
    int Charisma,
    bool KilledInAction,
    string? Alignment = null,
    string? Personality = null,
    string? IdealsAndGoals = null,
    string? Flaws = null,
    string? Languages = null,
    string? Appearance = null);

public class CharacterSheetDto
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }

    public int MaxHitPoints { get; set; }
    public int CurrentHitPoints { get; set; }
    public int TemporaryHitPoints { get; set; }

    public int ArmorClass { get; set; }
    public int TouchArmorClass { get; set; }
    public int FlatFootedArmorClass { get; set; }

    public int InitiativeBonus { get; set; }
    public int Speed { get; set; }

    public int BaseAttackBonus { get; set; }
    public int CombatManeuverBonus { get; set; }
    public int CombatManeuverDefense { get; set; }

    public int FortitudeSave { get; set; }
    public int ReflexSave { get; set; }
    public int WillSave { get; set; }

    public List<SkillEntryDto> Skills { get; set; } = [];

    public string? Feats { get; set; }
    public string? SpecialAbilities { get; set; }
    public string? Equipment { get; set; }
    public string? Spells { get; set; }
    public string? Notes { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class SkillEntryDto
{
    public string Name { get; set; } = string.Empty;
    public string AbilityKey { get; set; } = string.Empty;
    public bool IsClassSkill { get; set; }
    public int Ranks { get; set; }
    public int MiscBonus { get; set; }
    public bool TrainedOnly { get; set; }
}

public record CharacterSheetRequest(
    int MaxHitPoints,
    int CurrentHitPoints,
    int TemporaryHitPoints,
    int ArmorClass,
    int TouchArmorClass,
    int FlatFootedArmorClass,
    int InitiativeBonus,
    int Speed,
    int BaseAttackBonus,
    int CombatManeuverBonus,
    int CombatManeuverDefense,
    int FortitudeSave,
    int ReflexSave,
    int WillSave,
    List<SkillEntryRequest> Skills,
    string? Feats = null,
    string? SpecialAbilities = null,
    string? Equipment = null,
    string? Spells = null,
    string? Notes = null);

public record SkillEntryRequest(
    string Name,
    string AbilityKey,
    bool IsClassSkill,
    int Ranks,
    int MiscBonus,
    bool TrainedOnly);
