using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class CampaignRepository(AppDbContext context) : ICampaignRepository
{
    public async Task<IReadOnlyList<Campaign>> GetAllAsync()
        => await context.Campaigns
            .Include(c => c.Players)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();

    public async Task<IReadOnlyList<Campaign>> GetByPlayerAsync(Guid userId)
        => await context.Campaigns
            .Include(c => c.Players)
            .Where(c => c.Players.Any(p => p.Id == userId))
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();

    public async Task<Campaign?> GetByIdAsync(Guid id)
        => await context.Campaigns
            .Include(c => c.Players)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(Campaign campaign)
    {
        context.Campaigns.Add(campaign);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Campaign campaign)
    {
        context.Campaigns.Update(campaign);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var campaign = await context.Campaigns.FindAsync(id);
        if (campaign is not null)
        {
            context.Campaigns.Remove(campaign);
            await context.SaveChangesAsync();
        }
    }

    public async Task AddPlayerAsync(Guid campaignId, Guid userId)
    {
        var campaign = await context.Campaigns
            .Include(c => c.Players)
            .FirstOrDefaultAsync(c => c.Id == campaignId);
        var user = await context.Users.FindAsync(userId);

        if (campaign is null || user is null) return;
        if (campaign.Players.Any(p => p.Id == userId)) return;

        campaign.Players.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task RemovePlayerAsync(Guid campaignId, Guid userId)
    {
        var campaign = await context.Campaigns
            .Include(c => c.Players)
            .FirstOrDefaultAsync(c => c.Id == campaignId);

        if (campaign is null) return;

        var player = campaign.Players.FirstOrDefault(p => p.Id == userId);
        if (player is not null)
        {
            campaign.Players.Remove(player);
            await context.SaveChangesAsync();
        }
    }
}
