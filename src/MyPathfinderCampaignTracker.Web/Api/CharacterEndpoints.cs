using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class CharacterEndpoints
{
    public static IEndpointRouteBuilder MapCharacterEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/characters");

        group.MapGet("/my", async (ClaimsPrincipal user, ICharacterService characterService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();
            var characters = await characterService.GetByUserAsync(userId);
            return Results.Ok(characters);
        }).RequireAuthorization("ApiAuth");

        group.MapGet("/", async (Guid campaignId, ICharacterService characterService) =>
        {
            var characters = await characterService.GetByCampaignAsync(campaignId);
            return Results.Ok(characters);
        }).RequireAuthorization("ApiAuth");

        group.MapGet("/{id:guid}", async (Guid id, ICharacterService characterService) =>
        {
            var character = await characterService.GetByIdAsync(id);
            return character is null ? Results.NotFound() : Results.Ok(character);
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/", async (
            Guid campaignId,
            CharacterRequest request,
            ClaimsPrincipal user,
            ICharacterService characterService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Results.BadRequest("Name is required.");

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var character = await characterService.CreateAsync(campaignId, userId, request);
            return Results.Created($"/api/characters/{character.Id}", character);
        }).RequireAuthorization("ApiAuth");

        group.MapPut("/{id:guid}", async (
            Guid id,
            CharacterRequest request,
            ClaimsPrincipal user,
            ICharacterService characterService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Results.BadRequest("Name is required.");

            var existing = await characterService.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var isAdmin = user.IsInRole("Admin");
            if (existing.UserId != userId && !isAdmin)
                return Results.Forbid();

            var updated = await characterService.UpdateAsync(id, request);
            return updated ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ICharacterService characterService) =>
        {
            var existing = await characterService.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var isAdmin = user.IsInRole("Admin");
            if (existing.UserId != userId && !isAdmin)
                return Results.Forbid();

            var deleted = await characterService.DeleteAsync(id);
            return deleted ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth");

        group.MapPost("/{id:guid}/photo", async (
            Guid id,
            IFormFile photo,
            ClaimsPrincipal user,
            ICharacterService characterService) =>
        {
            var existing = await characterService.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var isAdmin = user.IsInRole("Admin");
            if (existing.UserId != userId && !isAdmin)
                return Results.Forbid();

            await using var stream = photo.OpenReadStream();
            var ok = await characterService.UploadPhotoAsync(id, stream);
            return ok ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization("ApiAuth")
          .DisableAntiforgery();

        group.MapGet("/{id:guid}/photo", async (
            Guid id,
            ICharacterService characterService) =>
        {
            var data = await characterService.GetPhotoAsync(id);
            if (data is null) return Results.NotFound();
            return Results.File(data, "image/png");
        }).RequireAuthorization("ApiAuth");

        return routes;
    }
}
