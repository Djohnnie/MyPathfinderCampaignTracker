using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Services;

public class GameSessionService(IGameSessionRepository gameSessionRepository) : IGameSessionService
{
    public async Task<IReadOnlyList<GameSessionDto>> GetByCampaignAsync(Guid campaignId)
    {
        var sessions = await gameSessionRepository.GetByCampaignAsync(campaignId);
        return sessions.Select(MapToDto).ToList();
    }

    public async Task<GameSessionDto?> GetNextByCampaignAsync(Guid campaignId)
    {
        var session = await gameSessionRepository.GetNextByCampaignAsync(campaignId, DateTime.UtcNow);
        return session is null ? null : MapToDto(session);
    }

    public async Task<GameSessionDto?> GetByIdAsync(Guid id)
    {
        var session = await gameSessionRepository.GetByIdAsync(id);
        return session is null ? null : MapToDto(session);
    }

    public async Task<GameSessionDto> CreateAsync(Guid campaignId, GameSessionRequest request)
    {
        var now = DateTime.UtcNow;
        var session = new GameSession
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            ScheduledAt = request.ScheduledAt,
            Location = request.Location,
            CreatedAt = now,
            UpdatedAt = now
        };

        await gameSessionRepository.AddAsync(session);
        return MapToDto(session);
    }

    public async Task<bool> UpdateAsync(Guid id, GameSessionRequest request)
    {
        var session = await gameSessionRepository.GetByIdAsync(id);
        if (session is null) return false;

        session.ScheduledAt = request.ScheduledAt;
        session.Location = request.Location;
        session.UpdatedAt = DateTime.UtcNow;

        await gameSessionRepository.UpdateAsync(session);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var session = await gameSessionRepository.GetByIdAsync(id);
        if (session is null) return false;

        await gameSessionRepository.DeleteAsync(id);
        return true;
    }

    private static GameSessionDto MapToDto(GameSession s) => new()
    {
        Id = s.Id,
        CampaignId = s.CampaignId,
        ScheduledAt = s.ScheduledAt,
        Location = s.Location,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}
