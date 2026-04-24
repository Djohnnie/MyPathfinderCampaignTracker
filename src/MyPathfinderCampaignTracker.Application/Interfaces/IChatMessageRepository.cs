using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IChatMessageRepository
{
    Task<IReadOnlyList<ChatMessage>> GetByCampaignAsync(Guid campaignId);
    Task AddAsync(ChatMessage message);
}
