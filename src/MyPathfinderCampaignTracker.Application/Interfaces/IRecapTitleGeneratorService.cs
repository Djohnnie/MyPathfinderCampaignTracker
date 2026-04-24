namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IRecapTitleGeneratorService
{
    Task<string> GenerateTitleAsync(string contents, CancellationToken cancellationToken = default);
}
