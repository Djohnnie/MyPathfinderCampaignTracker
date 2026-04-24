using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Infrastructure.Data;
using MyPathfinderCampaignTracker.Infrastructure.Repositories;
using MyPathfinderCampaignTracker.Infrastructure.Services;

namespace MyPathfinderCampaignTracker.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["SQL_CONNECTION_STRING"]
            ?? throw new InvalidOperationException(
                "SQL_CONNECTION_STRING environment variable is not set.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
