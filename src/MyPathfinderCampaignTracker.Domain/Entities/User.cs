namespace MyPathfinderCampaignTracker.Domain.Entities;

public class User
{
    public int SysId { get; set; }
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsApproved { get; set; }
    public bool IsDarkMode { get; set; }
    public Guid? FavoriteCampaignId { get; set; }
    public DateTime CreatedAt { get; set; }
}
