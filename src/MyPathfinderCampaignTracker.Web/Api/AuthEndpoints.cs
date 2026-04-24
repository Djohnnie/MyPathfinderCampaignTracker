using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Web.Services;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterRequest request, IUserService userService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return Results.BadRequest("Username and password are required.");

            var result = await userService.RegisterAsync(request.Username, request.Password);
            return result switch
            {
                Application.Models.RegisterResult.Success => Results.Ok(new { message = "Registration successful. Awaiting admin approval." }),
                Application.Models.RegisterResult.UserAlreadyExists => Results.Conflict("Username is already taken."),
                _ => Results.StatusCode(500)
            };
        });

        group.MapPost("/login", async (LoginRequest request, IUserService userService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return Results.BadRequest("Username and password are required.");

            var result = await userService.LoginAsync(request.Username, request.Password);
            if (!result.Success)
                return Results.Unauthorized();

            return Results.Ok(new { token = result.Token, user = result.User });
        });

        group.MapGet("/login-callback", async (string ticket, LoginTicketService ticketService, HttpContext ctx) =>
        {
            var data = ticketService.RedeemTicket(ticket);
            if (data is null)
                return Results.Redirect("/login?error=expired");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, data.UserId.ToString()),
                new(ClaimTypes.Name, data.Username),
                new(ClaimTypes.Role, data.IsAdmin ? "Admin" : "User"),
                new("access_token", data.Token),
                new("dark_mode", data.IsDarkMode ? "true" : "false")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Results.Redirect("/dashboard");
        });

        group.MapGet("/signout", async (HttpContext ctx) =>
        {
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/login");
        });

        group.MapPost("/logout", (HttpContext ctx) =>
        {
            ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Ok();
        }).RequireAuthorization("ApiAuth");

        return routes;
    }

    private record RegisterRequest(string Username, string Password);
    private record LoginRequest(string Username, string Password);
}
