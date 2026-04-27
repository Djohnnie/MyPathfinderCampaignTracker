using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class CharacterSheetEndpoints
{
    public static IEndpointRouteBuilder MapCharacterSheetEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/characters/{characterId:guid}/sheet");

        group.MapGet("/", async (
            Guid characterId,
            ICharacterSheetService sheetService,
            ICharacterService characterService,
            ClaimsPrincipal user) =>
        {
            var character = await characterService.GetByIdAsync(characterId);
            if (character is null) return Results.NotFound();

            var sheet = await sheetService.GetOrCreateAsync(characterId);
            return Results.Ok(sheet);
        }).RequireAuthorization("ApiAuth");

        group.MapPut("/", async (
            Guid characterId,
            CharacterSheetRequest request,
            ICharacterSheetService sheetService,
            ICharacterService characterService,
            ClaimsPrincipal user) =>
        {
            var character = await characterService.GetByIdAsync(characterId);
            if (character is null) return Results.NotFound();

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var isAdmin = user.IsInRole("Admin");
            if (character.UserId != userId && !isAdmin)
                return Results.Forbid();

            await sheetService.UpdateAsync(characterId, request);
            return Results.Ok();
        }).RequireAuthorization("ApiAuth");

        return routes;
    }
}
