namespace MyPathfinderCampaignTracker.Domain.Entities;

public enum ActivityType
{
    CharacterAdded,
    CharacterEdited,
    CharacterRemoved,
    RecapAdded,
    RecapEdited,
    RecapRemoved,
    ChatAdded,
    NoteAdded,
    NoteEdited,
    NoteRemoved,
    SessionAdded,
    SessionEdited,
    SessionRemoved,
    CampaignOpened
}

public class ActivityLog
{
    public int SysId { get; set; }
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public ActivityType ActivityType { get; set; }
    public string? SubjectName { get; set; }
    public DateTime OccurredAt { get; set; }
}
