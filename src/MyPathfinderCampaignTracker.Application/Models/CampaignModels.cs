namespace MyPathfinderCampaignTracker.Application.Models;

public class CampaignDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string ExtensiveInformation { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<UserDto> Players { get; set; } = [];
}

public record CampaignRequest(
    string Title,
    string Description,
    string? Link,
    string ExtensiveInformation,
    DateTime StartDate,
    DateTime? EndDate);

public record DescriptionUpdateRequest(string Description);
