using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Services;

public class ChatMessageService(IChatMessageRepository chatMessageRepository) : IChatMessageService
{
    public async Task<IReadOnlyList<ChatMessageDto>> GetByCampaignAsync(Guid campaignId)
    {
        var messages = await chatMessageRepository.GetByCampaignAsync(campaignId);
        return messages.Select(MapToDto).ToList();
    }

    public async Task<ChatMessageDto> SendAsync(Guid campaignId, Guid userId, string content)
    {
        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            UserId = userId,
            Content = content.Trim(),
            SentAt = DateTime.UtcNow
        };

        await chatMessageRepository.AddAsync(message);
        return new ChatMessageDto
        {
            Id = message.Id,
            CampaignId = message.CampaignId,
            UserId = message.UserId,
            Username = string.Empty,
            Content = message.Content,
            SentAt = message.SentAt
        };
    }

    private static ChatMessageDto MapToDto(ChatMessage m) => new()
    {
        Id = m.Id,
        CampaignId = m.CampaignId,
        UserId = m.UserId,
        Username = m.User?.Username ?? string.Empty,
        Content = m.Content,
        SentAt = m.SentAt
    };
}
