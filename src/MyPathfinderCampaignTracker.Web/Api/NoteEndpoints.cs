using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class NoteEndpoints
{
    public static IEndpointRouteBuilder MapNoteEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/campaigns/{campaignId:guid}/notes");

        group.MapGet("/", async (Guid campaignId, ICampaignNoteService noteService) =>
        {
            var notes = await noteService.GetByCampaignAsync(campaignId);
            return Results.Ok(notes);
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/", async (
            Guid campaignId,
            CampaignNoteRequest request,
            ClaimsPrincipal user,
            ICampaignNoteService noteService,
            ICampaignService campaignService,
            IActivityLogService activityLogService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var campaign = await campaignService.GetByIdAsync(campaignId);
            if (campaign is null) return Results.NotFound();

            var isAdmin = user.IsInRole("Admin");
            if (!isAdmin && !campaign.Players.Any(p => p.Id == userId))
                return Results.Forbid();

            if (string.IsNullOrWhiteSpace(request.Content))
                return Results.BadRequest("Content is required.");

            var note = await noteService.CreateAsync(campaignId, userId, request);
            try { await activityLogService.LogAsync(campaignId, userId, ActivityType.NoteAdded); } catch { }
            return Results.Created($"/api/campaigns/{campaignId}/notes/{note.Id}", note);
        }).RequireAuthorization("ApiAuth");

        group.MapPut("/{id:guid}", async (
            Guid campaignId,
            Guid id,
            CampaignNoteRequest request,
            ClaimsPrincipal user,
            ICampaignNoteService noteService,
            IActivityLogService activityLogService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var existing = await noteService.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();

            var isAdmin = user.IsInRole("Admin");
            if (!isAdmin && existing.UserId != userId)
                return Results.Forbid();

            if (string.IsNullOrWhiteSpace(request.Content))
                return Results.BadRequest("Content is required.");

            var updated = await noteService.UpdateAsync(id, request);
            if (updated) try { await activityLogService.LogAsync(campaignId, userId, ActivityType.NoteEdited); } catch { }
            return updated ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        group.MapDelete("/{id:guid}", async (
            Guid campaignId,
            Guid id,
            ClaimsPrincipal user,
            ICampaignNoteService noteService,
            IActivityLogService activityLogService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var existing = await noteService.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();

            var isAdmin = user.IsInRole("Admin");
            if (!isAdmin && existing.UserId != userId)
                return Results.Forbid();

            var deleted = await noteService.DeleteAsync(id);
            if (deleted) try { await activityLogService.LogAsync(campaignId, userId, ActivityType.NoteRemoved); } catch { }
            return deleted ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        return routes;
    }
}
