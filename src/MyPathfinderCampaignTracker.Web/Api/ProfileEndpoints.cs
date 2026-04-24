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

        return routes;
    }

    private record SetDarkModeRequest(bool IsDarkMode);
}
