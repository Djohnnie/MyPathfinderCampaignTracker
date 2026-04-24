using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class CharacterRepository(AppDbContext context) : ICharacterRepository
{
    public async Task<IReadOnlyList<Character>> GetByCampaignAsync(Guid campaignId)
        => await context.Characters
            .Include(c => c.User)
            .Where(c => c.CampaignId == campaignId)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<Character?> GetByIdAsync(Guid id)
        => await context.Characters
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(Character character)
    {
        context.Characters.Add(character);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Character character)
    {
        context.Characters.Update(character);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var character = await context.Characters.FindAsync(id);
        if (character is not null)
        {
            context.Characters.Remove(character);
            await context.SaveChangesAsync();
        }
    }
}
