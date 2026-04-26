using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<bool> AnyAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task SetFavoriteCampaignAsync(Guid userId, Guid? campaignId);
    Task<(Guid Id, string Title)?> GetFavoriteCampaignAsync(Guid userId);
}
