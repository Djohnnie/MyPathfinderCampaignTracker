using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class GameSessionEndpoints
{
    public static IEndpointRouteBuilder MapGameSessionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/campaigns/{campaignId:guid}/sessions");

        group.MapGet("/", async (Guid campaignId, IGameSessionService gameSessionService) =>
        {
            var sessions = await gameSessionService.GetByCampaignAsync(campaignId);
            return Results.Ok(sessions);
        }).RequireAuthorization("ApiAuth");

        group.MapGet("/next", async (Guid campaignId, IGameSessionService gameSessionService) =>
        {
            var session = await gameSessionService.GetNextByCampaignAsync(campaignId);
            return session is null ? Results.NoContent() : Results.Ok(session);
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/", async (
            Guid campaignId,
            GameSessionRequest request,
            ClaimsPrincipal user,
            IGameSessionService gameSessionService,
            ICampaignService campaignService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var campaign = await campaignService.GetByIdAsync(campaignId);
            if (campaign is null) return Results.NotFound();

            var isAdmin = user.IsInRole("Admin");
            if (!isAdmin && !campaign.Players.Any(p => p.Id == userId))
                return Results.Forbid();

            if (string.IsNullOrWhiteSpace(request.Location))
                return Results.BadRequest("Location is required.");

            var session = await gameSessionService.CreateAsync(campaignId, request);
            return Results.Created($"/api/campaigns/{campaignId}/sessions/{session.Id}", session);
        }).RequireAuthorization("ApiAuth");

        group.MapPut("/{id:guid}", async (
            Guid campaignId,
            Guid id,
            GameSessionRequest request,
            ClaimsPrincipal user,
            IGameSessionService gameSessionService,
            ICampaignService campaignService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var campaign = await campaignService.GetByIdAsync(campaignId);
            if (campaign is null) return Results.NotFound();

            var isAdmin = user.IsInRole("Admin");
            if (!isAdmin && !campaign.Players.Any(p => p.Id == userId))
                return Results.Forbid();

            if (string.IsNullOrWhiteSpace(request.Location))
                return Results.BadRequest("Location is required.");

            var updated = await gameSessionService.UpdateAsync(id, request);
            return updated ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        group.MapDelete("/{id:guid}", async (
            Guid campaignId,
            Guid id,
            ClaimsPrincipal user,
            IGameSessionService gameSessionService,
            ICampaignService campaignService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var campaign = await campaignService.GetByIdAsync(campaignId);
            if (campaign is null) return Results.NotFound();

            var isAdmin = user.IsInRole("Admin");
            if (!isAdmin && !campaign.Players.Any(p => p.Id == userId))
                return Results.Forbid();

            var deleted = await gameSessionService.DeleteAsync(id);
            return deleted ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        return routes;
    }
}
