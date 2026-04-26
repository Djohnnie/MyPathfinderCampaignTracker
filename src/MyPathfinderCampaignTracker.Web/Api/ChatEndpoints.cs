using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/campaigns/{campaignId:guid}/chat");

        group.MapGet("/", async (Guid campaignId, IChatMessageService chatService) =>
        {
            var messages = await chatService.GetByCampaignAsync(campaignId);
            return Results.Ok(messages);
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/", async (
            Guid campaignId,
            ChatMessageRequest request,
            ClaimsPrincipal user,
            IChatMessageService chatService,
            IActivityLogService activityLogService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return Results.BadRequest("Bericht mag niet leeg zijn.");

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var message = await chatService.SendAsync(campaignId, userId, request.Content);
            try { await activityLogService.LogAsync(campaignId, userId, ActivityType.ChatAdded); } catch { }
            return Results.Created($"/api/campaigns/{campaignId}/chat/{message.Id}", message);
        }).RequireAuthorization("ApiAuth");

        return routes;
    }
}
