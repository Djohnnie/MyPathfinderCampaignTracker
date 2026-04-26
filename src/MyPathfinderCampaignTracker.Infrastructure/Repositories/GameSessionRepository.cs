using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class GameSessionRepository(AppDbContext context) : IGameSessionRepository
{
    public async Task<IReadOnlyList<GameSession>> GetByCampaignAsync(Guid campaignId)
        => await context.GameSessions
            .Where(s => s.CampaignId == campaignId)
            .OrderBy(s => s.ScheduledAt)
            .ToListAsync();

    public async Task<GameSession?> GetByIdAsync(Guid id)
        => await context.GameSessions.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<GameSession?> GetNextByCampaignAsync(Guid campaignId, DateTime now)
        => await context.GameSessions
            .Where(s => s.CampaignId == campaignId && s.ScheduledAt >= now)
            .OrderBy(s => s.ScheduledAt)
            .FirstOrDefaultAsync();

    public async Task AddAsync(GameSession session)
    {
        context.GameSessions.Add(session);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(GameSession session)
    {
        context.GameSessions.Update(session);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var session = await context.GameSessions.FindAsync(id);
        if (session is not null)
        {
            context.GameSessions.Remove(session);
            await context.SaveChangesAsync();
        }
    }
}
