using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class RecapEndpoints
{
    public static IEndpointRouteBuilder MapRecapEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/recaps");

        group.MapGet("/my", async (ClaimsPrincipal user, IRecapService recapService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();
            var recaps = await recapService.GetByUserAsync(userId);
            return Results.Ok(recaps);
        }).RequireAuthorization("ApiAuth");

        group.MapGet("/", async (Guid campaignId, IRecapService recapService) =>
        {
            var recaps = await recapService.GetByCampaignAsync(campaignId);
            return Results.Ok(recaps);
        }).RequireAuthorization("ApiAuth");

        group.MapGet("/{id:guid}", async (Guid id, IRecapService recapService) =>
        {
            var recap = await recapService.GetByIdAsync(id);
            return recap is null ? Results.NotFound() : Results.Ok(recap);
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/", async (
            Guid campaignId,
            RecapRequest request,
            ClaimsPrincipal user,
            IRecapService recapService,
            IActivityLogService activityLogService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest("Title is required.");

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var recap = await recapService.CreateAsync(campaignId, userId, request);
            try { await activityLogService.LogAsync(campaignId, userId, ActivityType.RecapAdded, recap.Title); } catch { }
            return Results.Created($"/api/recaps/{recap.Id}", recap);
        }).RequireAuthorization("ApiAuth");

        group.MapPut("/{id:guid}", async (
            Guid id,
            RecapRequest request,
            ClaimsPrincipal user,
            IRecapService recapService,
            IActivityLogService activityLogService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest("Title is required.");

            var existing = await recapService.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var isAdmin = user.IsInRole("Admin");
            if (existing.UserId != userId && !isAdmin)
                return Results.Forbid();

            var updated = await recapService.UpdateAsync(id, request);
            if (updated) try { await activityLogService.LogAsync(existing.CampaignId, userId, ActivityType.RecapEdited, existing.Title); } catch { }
            return updated ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            IRecapService recapService,
            IActivityLogService activityLogService) =>
        {
            var existing = await recapService.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var isAdmin = user.IsInRole("Admin");
            if (existing.UserId != userId && !isAdmin)
                return Results.Forbid();

            var deleted = await recapService.DeleteAsync(id);
            if (deleted) try { await activityLogService.LogAsync(existing.CampaignId, userId, ActivityType.RecapRemoved, existing.Title); } catch { }
            return deleted ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/generate-title", async (
            RecapGenerateTitleRequest request,
            IRecapTitleGeneratorService titleGeneratorService,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Contents))
                return Results.BadRequest("Contents is required.");

            var title = await titleGeneratorService.GenerateTitleAsync(request.Contents, ct);
            return Results.Ok(new RecapGenerateTitleResponse(title));
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/format-contents", async (
            RecapFormatContentsRequest request,
            IRecapFormatterService formatterService,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Contents))
                return Results.BadRequest("Contents is required.");

            var formatted = await formatterService.FormatContentsAsync(request.Contents, ct);
            return Results.Ok(new RecapFormatContentsResponse(formatted));
        }).RequireAuthorization("ApiAuth");

        return routes;
    }
}
