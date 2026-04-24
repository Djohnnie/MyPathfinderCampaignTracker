namespace MyPathfinderCampaignTracker.Application.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsApproved { get; set; }
    public bool IsDarkMode { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum RegisterResult
{
    Success,
    UserAlreadyExists
}

public class LoginResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public UserDto? User { get; set; }
    public string? Error { get; set; }

    public static LoginResult Ok(string token, UserDto user) =>
        new() { Success = true, Token = token, User = user };

    public static LoginResult Fail(string error) =>
        new() { Success = false, Error = error };
}

public enum ApproveResult
{
    Success,
    UserNotFound,
    AlreadyApproved
}

public enum PromoteResult
{
    Success,
    UserNotFound,
    AlreadyInRole
}
