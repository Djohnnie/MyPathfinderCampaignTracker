using System.Security.Claims;
using MyPathfinderCampaignTracker.Application.Interfaces;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class ProfileEndpoints
{
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/profile");

        group.MapPut("/darkmode", async (SetDarkModeRequest request, ClaimsPrincipal user, IUserService userService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            await userService.SetDarkModeAsync(userId, request.IsDarkMode);
            return Results.Ok();
        }).RequireAuthorization("ApiAuth");

        group.MapGet("/favorite-campaign", async (ClaimsPrincipal user, IUserService userService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var favorite = await userService.GetFavoriteCampaignAsync(userId);
            return Results.Ok(new FavoriteCampaignResponse(favorite?.Id, favorite?.Title));
        }).RequireAuthorization("ApiAuth");

        group.MapPut("/favorite-campaign", async (SetFavoriteCampaignRequest request, ClaimsPrincipal user, IUserService userService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            await userService.SetFavoriteCampaignAsync(userId, request.CampaignId);
            return Results.Ok();
        }).RequireAuthorization("ApiAuth");

        return routes;
    }

    private record SetDarkModeRequest(bool IsDarkMode);
    private record SetFavoriteCampaignRequest(Guid? CampaignId);
    private record FavoriteCampaignResponse(Guid? Id, string? Title);
}
