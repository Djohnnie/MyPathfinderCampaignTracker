using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using MyPathfinderCampaignTracker.Application;
using MyPathfinderCampaignTracker.Infrastructure;
using MyPathfinderCampaignTracker.Infrastructure.Data;
using MyPathfinderCampaignTracker.Web.Api;
using MyPathfinderCampaignTracker.Web.Components;
using MyPathfinderCampaignTracker.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.Cookie.Name = "PathfinderAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiAuth", policy =>
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser());
    options.AddPolicy("ApiAdmin", policy =>
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
              .RequireRole("Admin"));
});

builder.Services.AddHttpClient("ApiClient", client =>
{
    var apiBaseUrl = builder.Configuration["API_BASE_URL"] ?? "https://localhost:7279";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<ApiClient>();
builder.Services.AddSingleton<LoginTicketService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapAuthEndpoints();
app.MapUserManagementEndpoints();
app.MapProfileEndpoints();
app.MapCampaignEndpoints();
app.MapCharacterEndpoints();
app.MapRecapEndpoints();
app.MapChatEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
