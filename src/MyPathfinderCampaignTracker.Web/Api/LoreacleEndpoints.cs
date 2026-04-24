using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class LoreacleEndpoints
{
    public static IEndpointRouteBuilder MapLoreacleEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/campaigns/{campaignId:guid}/loreacle",
            async (
                Guid campaignId,
                LoreacleRequest request,
                ICampaignService campaignService,
                ICharacterService characterService,
                IRecapService recapService,
                ILoreacleService loreacleService,
                CancellationToken ct) =>
            {
                var campaign = await campaignService.GetByIdAsync(campaignId);
                if (campaign is null)
                    return Results.NotFound();

                var characters = await characterService.GetByCampaignAsync(campaignId);
                var recaps = await recapService.GetByCampaignAsync(campaignId);

                var recapSummaries = recaps
                    .OrderByDescending(r => r.Number)
                    .Take(10)
                    .Select(r => $"Recap {r.Number} – {r.Title}")
                    .ToList();

                var characterSummaries = characters
                    .Select(c => $"{c.Name} ({c.Race} {c.CharacterClass}, Level {c.Level})" +
                                 (c.KilledInAction ? " †" : ""))
                    .ToList();

                var reply = await loreacleService.ChatAsync(
                    request.UserMessage,
                    campaign.Title,
                    campaign.Description,
                    recapSummaries,
                    characterSummaries,
                    request.History,
                    ct);

                return Results.Ok(new LoreacleResponse(reply));
            })
            .RequireAuthorization("ApiAuth");

        return routes;
    }
}
