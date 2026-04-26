using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class CampaignNoteRepository(AppDbContext context) : ICampaignNoteRepository
{
    public async Task<IReadOnlyList<CampaignNote>> GetByCampaignAsync(Guid campaignId)
        => await context.CampaignNotes
            .Include(n => n.User)
            .Where(n => n.CampaignId == campaignId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task<CampaignNote?> GetByIdAsync(Guid id)
        => await context.CampaignNotes
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id);

    public async Task AddAsync(CampaignNote note)
    {
        context.CampaignNotes.Add(note);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CampaignNote note)
    {
        context.CampaignNotes.Update(note);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var note = await context.CampaignNotes.FindAsync(id);
        if (note is not null)
        {
            context.CampaignNotes.Remove(note);
            await context.SaveChangesAsync();
        }
    }
}
