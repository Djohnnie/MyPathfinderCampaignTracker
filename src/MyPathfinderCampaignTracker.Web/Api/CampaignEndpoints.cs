using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class CampaignEndpoints
{
    public static IEndpointRouteBuilder MapCampaignEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/campaigns");

        group.MapGet("/", async (ICampaignService campaignService) =>
        {
            var campaigns = await campaignService.GetAllAsync();
            return Results.Ok(campaigns);
        }).RequireAuthorization("ApiAuth");

        group.MapGet("/{id:guid}", async (Guid id, ICampaignService campaignService) =>
        {
            var campaign = await campaignService.GetByIdAsync(id);
            return campaign is null ? Results.NotFound() : Results.Ok(campaign);
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/", async (CampaignRequest request, ICampaignService campaignService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest("Title is required.");

            var campaign = await campaignService.CreateAsync(request);
            return Results.Created($"/api/campaigns/{campaign.Id}", campaign);
        }).RequireAuthorization("ApiAdmin");

        group.MapPut("/{id:guid}", async (Guid id, CampaignRequest request, ICampaignService campaignService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest("Title is required.");

            var updated = await campaignService.UpdateAsync(id, request);
            return updated ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAdmin");

        group.MapDelete("/{id:guid}", async (Guid id, ICampaignService campaignService) =>
        {
            var deleted = await campaignService.DeleteAsync(id);
            return deleted ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAdmin");

        group.MapPost("/{id:guid}/players/{userId:guid}", async (Guid id, Guid userId, ICampaignService campaignService) =>
        {
            var result = await campaignService.AddPlayerAsync(id, userId);
            return result ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAdmin");

        group.MapDelete("/{id:guid}/players/{userId:guid}", async (Guid id, Guid userId, ICampaignService campaignService) =>
        {
            var result = await campaignService.RemovePlayerAsync(id, userId);
            return result ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAdmin");

        return routes;
    }
}
