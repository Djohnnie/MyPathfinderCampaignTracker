namespace MyPathfinderCampaignTracker.Domain.Entities;

public class CharacterSheet
{
    public int SysId { get; set; }
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    // Hit points
    public int MaxHitPoints { get; set; }
    public int CurrentHitPoints { get; set; }
    public int TemporaryHitPoints { get; set; }

    // Armor
    public int ArmorClass { get; set; } = 10;
    public int TouchArmorClass { get; set; } = 10;
    public int FlatFootedArmorClass { get; set; } = 10;

    // Movement and initiative
    public int InitiativeBonus { get; set; }
    public int Speed { get; set; } = 30;

    // Combat
    public int BaseAttackBonus { get; set; }
    public int CombatManeuverBonus { get; set; }
    public int CombatManeuverDefense { get; set; } = 10;

    // Saving throws (total bonus including ability modifier and misc)
    public int FortitudeSave { get; set; }
    public int ReflexSave { get; set; }
    public int WillSave { get; set; }

    // Skills stored as JSON: array of SkillEntry objects
    public string? SkillsJson { get; set; }

    // Free-text sections
    public string? Feats { get; set; }
    public string? SpecialAbilities { get; set; }
    public string? Equipment { get; set; }
    public string? Spells { get; set; }
    public string? Notes { get; set; }

    public DateTime UpdatedAt { get; set; }
}
