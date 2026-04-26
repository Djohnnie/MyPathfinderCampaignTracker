using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Application.Services;

public class CampaignNoteService(ICampaignNoteRepository noteRepository) : ICampaignNoteService
{
    public async Task<IReadOnlyList<CampaignNoteDto>> GetByCampaignAsync(Guid campaignId)
    {
        var notes = await noteRepository.GetByCampaignAsync(campaignId);
        return notes.Select(MapToDto).ToList();
    }

    public async Task<CampaignNoteDto?> GetByIdAsync(Guid id)
    {
        var note = await noteRepository.GetByIdAsync(id);
        return note is null ? null : MapToDto(note);
    }

    public async Task<CampaignNoteDto> CreateAsync(Guid campaignId, Guid userId, CampaignNoteRequest request)
    {
        var now = DateTime.UtcNow;
        var note = new CampaignNote
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            UserId = userId,
            Content = request.Content,
            CreatedAt = now,
            UpdatedAt = now
        };

        await noteRepository.AddAsync(note);
        return MapToDto(note);
    }

    public async Task<bool> UpdateAsync(Guid id, CampaignNoteRequest request)
    {
        var note = await noteRepository.GetByIdAsync(id);
        if (note is null) return false;

        note.Content = request.Content;
        note.UpdatedAt = DateTime.UtcNow;

        await noteRepository.UpdateAsync(note);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var note = await noteRepository.GetByIdAsync(id);
        if (note is null) return false;

        await noteRepository.DeleteAsync(id);
        return true;
    }

    private static CampaignNoteDto MapToDto(CampaignNote n) => new()
    {
        Id = n.Id,
        CampaignId = n.CampaignId,
        UserId = n.UserId,
        AuthorUsername = n.User?.Username ?? string.Empty,
        Content = n.Content,
        CreatedAt = n.CreatedAt,
        UpdatedAt = n.UpdatedAt
    };
}
