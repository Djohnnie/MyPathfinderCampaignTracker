using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;

public sealed class NoOpLoreacleService : ILoreacleService
{
    public Task<(string Reply, bool HistoryCleared)> ChatAsync(
        string userMessage,
        string campaignTitle,
        string campaignDescription,
        Guid campaignId,
        Guid userId,
        IReadOnlyList<string> recapSummaries,
        IReadOnlyList<string> characterSummaries,
        IReadOnlyList<string> sessionSummaries,
        IReadOnlyList<string> noteSummaries,
        IReadOnlyList<LoreacleMessageDto> history,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<(string, bool)>(
            ("Loreacle is not configured. Add AzureOpenAI settings to appsettings.json.", false));
    }

    public Task<string> CompactAsync(
        string campaignTitle,
        string? previousCompaction,
        IReadOnlyList<LoreacleMessageDto> messagesToCompact,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult("(Compaction not available — Loreacle is not configured.)");
    }
}
