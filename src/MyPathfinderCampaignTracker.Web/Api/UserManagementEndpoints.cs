using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class UserManagementEndpoints
{
    public static IEndpointRouteBuilder MapUserManagementEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/users").RequireAuthorization("ApiAdmin");

        group.MapGet("/", async (IUserService userService) =>
        {
            var users = await userService.GetAllUsersAsync();
            return Results.Ok(users);
        });

        group.MapPut("/{id:guid}/approve", async (Guid id, IUserService userService) =>
        {
            var result = await userService.ApproveUserAsync(id);
            return result switch
            {
                ApproveResult.Success => Results.Ok(),
                ApproveResult.UserNotFound => Results.NotFound(),
                ApproveResult.AlreadyApproved => Results.Conflict("User is already approved."),
                _ => Results.StatusCode(500)
            };
        });

        group.MapPut("/{id:guid}/promote", async (Guid id, IUserService userService) =>
        {
            var result = await userService.PromoteUserAsync(id);
            return result switch
            {
                PromoteResult.Success => Results.Ok(),
                PromoteResult.UserNotFound => Results.NotFound(),
                PromoteResult.AlreadyInRole => Results.Conflict("User is already an admin."),
                _ => Results.StatusCode(500)
            };
        });

        group.MapPut("/{id:guid}/demote", async (Guid id, IUserService userService) =>
        {
            var result = await userService.DemoteUserAsync(id);
            return result switch
            {
                PromoteResult.Success => Results.Ok(),
                PromoteResult.UserNotFound => Results.NotFound(),
                PromoteResult.AlreadyInRole => Results.Conflict("User is not an admin."),
                _ => Results.StatusCode(500)
            };
        });

        group.MapDelete("/{id:guid}", async (Guid id, IUserService userService) =>
        {
            var deleted = await userService.DeleteUserAsync(id);
            return deleted ? Results.Ok() : Results.NotFound();
        });

        return routes;
    }
}
