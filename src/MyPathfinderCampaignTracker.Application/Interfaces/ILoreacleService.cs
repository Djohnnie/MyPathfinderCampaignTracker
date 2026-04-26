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
        IReadOnlyList<string> sessionSummaries,
        IReadOnlyList<string> noteSummaries,
        IReadOnlyList<LoreacleMessageDto> history,
        CancellationToken cancellationToken = default);

    Task<string> CompactAsync(
        string campaignTitle,
        string? previousCompaction,
        IReadOnlyList<LoreacleMessageDto> messagesToCompact,
        CancellationToken cancellationToken = default);
}
