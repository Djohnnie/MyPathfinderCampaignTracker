using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ILoreacleService
{
    Task<string> ChatAsync(
        string userMessage,
        string campaignTitle,
        string campaignDescription,
        IReadOnlyList<string> recapSummaries,
        IReadOnlyList<string> characterSummaries,
        IReadOnlyList<LoreacleMessageDto> history,
        CancellationToken cancellationToken = default);
}
