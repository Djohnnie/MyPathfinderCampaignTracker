using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Services;

public class RecapService(IRecapRepository recapRepository) : IRecapService
{
    public async Task<IReadOnlyList<RecapDto>> GetByCampaignAsync(Guid campaignId)
    {
        var recaps = await recapRepository.GetByCampaignAsync(campaignId);
        return recaps.Select(r => MapToDto(r)).ToList();
    }

    public async Task<IReadOnlyList<RecapDto>> GetByUserAsync(Guid userId)
    {
        var recaps = await recapRepository.GetByUserAsync(userId);
        return recaps.Select(r => MapToDto(r, includeCampaignName: true)).ToList();
    }

    public async Task<RecapDto?> GetByIdAsync(Guid id)
    {
        var recap = await recapRepository.GetByIdAsync(id);
        return recap is null ? null : MapToDto(recap);
    }

    public async Task<RecapDto> CreateAsync(Guid campaignId, Guid userId, RecapRequest request)
    {
        var now = DateTime.UtcNow;
        var nextNumber = await recapRepository.GetMaxNumberAsync(campaignId) + 1;

        var recap = new Recap
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            UserId = userId,
            Number = nextNumber,
            Date = request.Date,
            Title = request.Title,
            Contents = request.Contents,
            CreatedAt = now,
            UpdatedAt = now
        };

        await recapRepository.AddAsync(recap);
        return MapToDto(recap);
    }

    public async Task<bool> UpdateAsync(Guid id, RecapRequest request)
    {
        var recap = await recapRepository.GetByIdAsync(id);
        if (recap is null) return false;

        recap.Date = request.Date;
        recap.Title = request.Title;
        recap.Contents = request.Contents;
        recap.UpdatedAt = DateTime.UtcNow;

        await recapRepository.UpdateAsync(recap);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var recap = await recapRepository.GetByIdAsync(id);
        if (recap is null) return false;

        await recapRepository.DeleteAsync(id);
        return true;
    }

    private static RecapDto MapToDto(Recap r, bool includeCampaignName = false) => new()
    {
        Id = r.Id,
        CampaignId = r.CampaignId,
        CampaignName = includeCampaignName ? (r.Campaign?.Title ?? string.Empty) : string.Empty,
        UserId = r.UserId,
        AuthorUsername = r.User?.Username ?? string.Empty,
        Number = r.Number,
        Date = r.Date,
        Title = r.Title,
        Contents = r.Contents,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
