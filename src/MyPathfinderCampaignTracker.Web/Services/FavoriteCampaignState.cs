namespace MyPathfinderCampaignTracker.Web.Services;

public class FavoriteCampaignState
{
    public Guid? CampaignId { get; private set; }
    public string? CampaignTitle { get; private set; }
    public bool IsLoaded { get; private set; }

    public event Action? OnChange;

    public void Set(Guid? id, string? title)
    {
        CampaignId = id;
        CampaignTitle = title;
        IsLoaded = true;
        OnChange?.Invoke();
    }
}
