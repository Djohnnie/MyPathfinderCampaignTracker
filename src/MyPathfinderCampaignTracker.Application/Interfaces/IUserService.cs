using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Application.Interfaces;

public interface IUserService
{
    Task<RegisterResult> RegisterAsync(string username, string password);
    Task<LoginResult> LoginAsync(string username, string password);
    Task<IReadOnlyList<UserDto>> GetAllUsersAsync();
    Task<ApproveResult> ApproveUserAsync(Guid userId);
    Task<PromoteResult> PromoteUserAsync(Guid userId);
    Task<PromoteResult> DemoteUserAsync(Guid userId);
    Task<bool> DeleteUserAsync(Guid userId);
    Task SetDarkModeAsync(Guid userId, bool isDarkMode);
}
