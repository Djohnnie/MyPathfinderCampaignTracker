namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IRecapFormatterService
{
    Task<string> FormatContentsAsync(string contents, CancellationToken cancellationToken = default);
}
