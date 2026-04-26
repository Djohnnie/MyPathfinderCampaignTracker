namespace MyPathfinderCampaignTracker.Web.Helpers;

public static class StringExtensions
{
    public static string CapFirst(this string? value)
    {
        if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
        return char.ToUpper(value[0]) + value[1..];
    }
}
