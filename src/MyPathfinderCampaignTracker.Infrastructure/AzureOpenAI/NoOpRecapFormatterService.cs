using MyPathfinderCampaignTracker.Application.Interfaces;

namespace MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;

public sealed class NoOpRecapFormatterService : IRecapFormatterService
{
    public Task<string> FormatContentsAsync(string contents, CancellationToken cancellationToken = default)
        => Task.FromResult(contents);
}
