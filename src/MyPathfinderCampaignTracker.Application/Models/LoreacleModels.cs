namespace MyPathfinderCampaignTracker.Application.Models;

public record LoreacleMessageDto(bool IsUser, string Content);

public record LoreacleRequest(string UserMessage);

public record LoreacleResponse(string Reply);
