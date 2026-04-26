using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id) =>
        await dbContext.Users.FindAsync(id);

    public async Task<User?> GetByUsernameAsync(string username) =>
        await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<IReadOnlyList<User>> GetAllAsync() =>
        await dbContext.Users.OrderBy(u => u.CreatedAt).ToListAsync();

    public async Task<bool> AnyAsync() =>
        await dbContext.Users.AnyAsync();

    public async Task AddAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user is null) return false;

        // Remove FK-restricted child rows manually (all use DeleteBehavior.Restrict)
        await dbContext.Characters.Where(c => c.UserId == id).ExecuteDeleteAsync();
        await dbContext.Recaps.Where(r => r.UserId == id).ExecuteDeleteAsync();
        await dbContext.ChatMessages.Where(m => m.UserId == id).ExecuteDeleteAsync();
        await dbContext.CampaignNotes.Where(n => n.UserId == id).ExecuteDeleteAsync();

        // Remove from CampaignUsers join table
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM [CampaignUsers] WHERE [PlayersId] = {id}");

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task SetFavoriteCampaignAsync(Guid userId, Guid? campaignId) =>
        await dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.FavoriteCampaignId, campaignId));

    public async Task<(Guid Id, string Title)?> GetFavoriteCampaignAsync(Guid userId)
    {
        var user = await dbContext.Users.FindAsync(userId);
        if (user?.FavoriteCampaignId is null) return null;

        var campaign = await dbContext.Campaigns.FindAsync(user.FavoriteCampaignId);
        if (campaign is null) return null;

        return (campaign.Id, campaign.Title);
    }
}