using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IGameSessionService
{
    Task<IReadOnlyList<GameSessionDto>> GetByCampaignAsync(Guid campaignId);
    Task<GameSessionDto?> GetNextByCampaignAsync(Guid campaignId);
    Task<GameSessionDto?> GetByIdAsync(Guid id);
    Task<GameSessionDto> CreateAsync(Guid campaignId, GameSessionRequest request);
    Task<bool> UpdateAsync(Guid id, GameSessionRequest request);
    Task<bool> DeleteAsync(Guid id);
}
