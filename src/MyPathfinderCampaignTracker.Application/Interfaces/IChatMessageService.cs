using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IChatMessageService
{
    Task<IReadOnlyList<ChatMessageDto>> GetByCampaignAsync(Guid campaignId);
    Task<ChatMessageDto> SendAsync(Guid campaignId, Guid userId, string content);
}
