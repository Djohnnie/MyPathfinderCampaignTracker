using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class RecapEndpoints
{
    public static IEndpointRouteBuilder MapRecapEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/recaps");

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
            IRecapService recapService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest("Title is required.");

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var recap = await recapService.CreateAsync(campaignId, userId, request);
            return Results.Created($"/api/recaps/{recap.Id}", recap);
        }).RequireAuthorization("ApiAuth");

        group.MapPut("/{id:guid}", async (
            Guid id,
            RecapRequest request,
            ClaimsPrincipal user,
            IRecapService recapService) =>
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
            return updated ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            IRecapService recapService) =>
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
            return deleted ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        return routes;
    }
}
