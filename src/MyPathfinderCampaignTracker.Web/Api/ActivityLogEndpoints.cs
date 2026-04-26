using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class ActivityLogEndpoints
{
    public static IEndpointRouteBuilder MapActivityLogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/campaigns/{campaignId:guid}/activity");

        group.MapGet("/", async (Guid campaignId, IActivityLogService activityLogService) =>
        {
            var logs = await activityLogService.GetRecentAsync(campaignId, 10);
            return Results.Ok(logs);
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/open", async (
            Guid campaignId,
            ClaimsPrincipal user,
            IActivityLogService activityLogService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            try { await activityLogService.LogAsync(campaignId, userId, ActivityType.CampaignOpened); }
            catch { /* swallow: logging must not break navigation */ }

            return Results.Ok();
        }).RequireAuthorization("ApiAuth");

        return routes;
    }
}
