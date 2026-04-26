using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Services;

public class CampaignService(ICampaignRepository campaignRepository) : ICampaignService
{
    public async Task<IReadOnlyList<CampaignDto>> GetAllAsync()
    {
        var campaigns = await campaignRepository.GetAllAsync();
        return campaigns.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<CampaignDto>> GetByPlayerAsync(Guid userId)
    {
        var campaigns = await campaignRepository.GetByPlayerAsync(userId);
        return campaigns.Select(MapToDto).ToList();
    }

    public async Task<CampaignDto?> GetByIdAsync(Guid id)
    {
        var campaign = await campaignRepository.GetByIdAsync(id);
        return campaign is null ? null : MapToDto(campaign);
    }

    public async Task<CampaignDto> CreateAsync(CampaignRequest request)
    {
        var now = DateTime.UtcNow;
        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Link = request.Link,
            ExtensiveInformation = request.ExtensiveInformation,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        await campaignRepository.AddAsync(campaign);
        return MapToDto(campaign);
    }

    public async Task<bool> UpdateAsync(Guid id, CampaignRequest request)
    {
        var campaign = await campaignRepository.GetByIdAsync(id);
        if (campaign is null) return false;

        campaign.Title = request.Title;
        campaign.Description = request.Description;
        campaign.Link = request.Link;
        campaign.ExtensiveInformation = request.ExtensiveInformation;
        campaign.StartDate = request.StartDate;
        campaign.EndDate = request.EndDate;
        campaign.UpdatedAt = DateTime.UtcNow;

        await campaignRepository.UpdateAsync(campaign);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var campaign = await campaignRepository.GetByIdAsync(id);
        if (campaign is null) return false;

        await campaignRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> AddPlayerAsync(Guid campaignId, Guid userId)
    {
        var campaign = await campaignRepository.GetByIdAsync(campaignId);
        if (campaign is null) return false;
        if (campaign.Players.Any(p => p.Id == userId)) return true;

        await campaignRepository.AddPlayerAsync(campaignId, userId);

        campaign.UpdatedAt = DateTime.UtcNow;
        await campaignRepository.UpdateAsync(campaign);
        return true;
    }

    public async Task<bool> RemovePlayerAsync(Guid campaignId, Guid userId)
    {
        var campaign = await campaignRepository.GetByIdAsync(campaignId);
        if (campaign is null) return false;

        await campaignRepository.RemovePlayerAsync(campaignId, userId);

        campaign.UpdatedAt = DateTime.UtcNow;
        await campaignRepository.UpdateAsync(campaign);
        return true;
    }

    public async Task<bool> UpdateDescriptionAsync(Guid campaignId, string description)
    {
        var campaign = await campaignRepository.GetByIdAsync(campaignId);
        if (campaign is null) return false;

        campaign.Description = description;
        campaign.UpdatedAt = DateTime.UtcNow;
        await campaignRepository.UpdateAsync(campaign);
        return true;
    }

    private static CampaignDto MapToDto(Campaign c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        Link = c.Link,
        ExtensiveInformation = c.ExtensiveInformation,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        Players = c.Players.Select(p => new UserDto
        {
            Id = p.Id,
            Username = p.Username,
            IsAdmin = p.IsAdmin,
            IsApproved = p.IsApproved,
            IsDarkMode = p.IsDarkMode,
            CreatedAt = p.CreatedAt
        }).ToList()
    };
}
