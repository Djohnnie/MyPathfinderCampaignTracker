using Microsoft.EntityFrameworkCore;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;
using MyPathfinderCampaignTracker.Infrastructure.Data;

namespace MyPathfinderCampaignTracker.Infrastructure.Repositories;

public class ChatMessageRepository(AppDbContext context) : IChatMessageRepository
{
    public async Task<IReadOnlyList<ChatMessage>> GetByCampaignAsync(Guid campaignId)
    {
        return await context.ChatMessages
            .Where(m => m.CampaignId == campaignId)
            .Include(m => m.User)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task AddAsync(ChatMessage message)
    {
        context.ChatMessages.Add(message);
        await context.SaveChangesAsync();
    }
}
