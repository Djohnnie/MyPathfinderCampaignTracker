using MyPathfinderCampaignTracker.Application.Interfaces;

namespace MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;

public sealed class NoOpRecapTitleGeneratorService : IRecapTitleGeneratorService
{
    public Task<string> GenerateTitleAsync(string contents, CancellationToken cancellationToken = default)
        => Task.FromResult(string.Empty);
}
