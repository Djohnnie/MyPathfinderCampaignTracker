using MyPathfinderCampaignTracker.Application.Interfaces;

namespace MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;

public sealed class NoOpTranslationService : ITranslationService
{
    public Task<string?> TranslateToNlAsync(string text, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);
}
