using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class TranslationEndpoints
{
    public static IEndpointRouteBuilder MapTranslationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/translate");

        group.MapPost("/", async (TranslationRequest request, ITranslationService translationService, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return Results.BadRequest("Text is required.");

            var translated = await translationService.TranslateToNlAsync(request.Text, ct);
            return Results.Ok(new TranslationResponse(translated));
        }).RequireAuthorization("ApiAuth");

        return routes;
    }
}
