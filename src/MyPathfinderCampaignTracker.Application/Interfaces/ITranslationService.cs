namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface ITranslationService
{
    Task<string?> TranslateToNlAsync(string text, CancellationToken cancellationToken = default);
}
