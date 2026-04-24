using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IUserService
{
    public async Task<RegisterResult> RegisterAsync(string username, string password)
    {
        if (await userRepository.GetByUsernameAsync(username) is not null)
            return RegisterResult.UserAlreadyExists;

        var isFirstUser = !await userRepository.AnyAsync();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = passwordHasher.Hash(password),
            IsAdmin = isFirstUser,
            IsApproved = isFirstUser,
            CreatedAt = DateTime.UtcNow
        };

        await userRepository.AddAsync(user);
        return RegisterResult.Success;
    }

    public async Task<LoginResult> LoginAsync(string username, string password)
    {
        var user = await userRepository.GetByUsernameAsync(username);
        if (user is null || !passwordHasher.Verify(password, user.PasswordHash))
            return LoginResult.Fail("Invalid username or password.");

        if (!user.IsApproved)
            return LoginResult.Fail("Your account is awaiting admin approval.");

        var token = tokenService.GenerateToken(user);
        return LoginResult.Ok(token, MapToDto(user));
    }

    public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<ApproveResult> ApproveUserAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null) return ApproveResult.UserNotFound;
        if (user.IsApproved) return ApproveResult.AlreadyApproved;

        user.IsApproved = true;
        await userRepository.UpdateAsync(user);
        return ApproveResult.Success;
    }

    public async Task<PromoteResult> PromoteUserAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null) return PromoteResult.UserNotFound;
        if (user.IsAdmin) return PromoteResult.AlreadyInRole;

        user.IsAdmin = true;
        await userRepository.UpdateAsync(user);
        return PromoteResult.Success;
    }

    public async Task<PromoteResult> DemoteUserAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null) return PromoteResult.UserNotFound;
        if (!user.IsAdmin) return PromoteResult.AlreadyInRole;

        user.IsAdmin = false;
        await userRepository.UpdateAsync(user);
        return PromoteResult.Success;
    }

    public async Task SetDarkModeAsync(Guid userId, bool isDarkMode)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null) return;

        user.IsDarkMode = isDarkMode;
        await userRepository.UpdateAsync(user);
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        IsAdmin = user.IsAdmin,
        IsApproved = user.IsApproved,
        IsDarkMode = user.IsDarkMode,
        CreatedAt = user.CreatedAt
    };
}
