namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IImageProcessingService
{
    Task<byte[]> CreateCircularAvatarAsync(Stream inputStream);
}
