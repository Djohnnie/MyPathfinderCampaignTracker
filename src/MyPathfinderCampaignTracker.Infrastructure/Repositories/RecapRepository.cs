using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class RecapRepository(AppDbContext context) : IRecapRepository
{
    public async Task<IReadOnlyList<Recap>> GetByCampaignAsync(Guid campaignId)
        => await context.Recaps
            .Include(r => r.User)
            .Where(r => r.CampaignId == campaignId)
            .OrderByDescending(r => r.Number)
            .ToListAsync();

    public async Task<IReadOnlyList<Recap>> GetByUserAsync(Guid userId)
        => await context.Recaps
            .Include(r => r.User)
            .Include(r => r.Campaign)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Date)
            .ThenByDescending(r => r.Number)
            .ToListAsync();

    public async Task<Recap?> GetByIdAsync(Guid id)
        => await context.Recaps
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<int> GetMaxNumberAsync(Guid campaignId)
        => await context.Recaps
            .Where(r => r.CampaignId == campaignId)
            .Select(r => (int?)r.Number)
            .MaxAsync() ?? 0;

    public async Task AddAsync(Recap recap)
    {
        context.Recaps.Add(recap);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Recap recap)
    {
        context.Recaps.Update(recap);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var recap = await context.Recaps.FindAsync(id);
        if (recap is not null)
        {
            context.Recaps.Remove(recap);
            await context.SaveChangesAsync();
        }
    }
}
