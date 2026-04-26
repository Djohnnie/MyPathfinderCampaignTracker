using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IGameSessionRepository
{
    Task<IReadOnlyList<GameSession>> GetByCampaignAsync(Guid campaignId);
    Task<GameSession?> GetByIdAsync(Guid id);
    Task<GameSession?> GetNextByCampaignAsync(Guid campaignId, DateTime now);
    Task AddAsync(GameSession session);
    Task UpdateAsync(GameSession session);
    Task DeleteAsync(Guid id);
}
