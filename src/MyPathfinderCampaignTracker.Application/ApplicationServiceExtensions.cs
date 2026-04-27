using Microsoft.Extensions.DependencyInjection;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Services;

namespace MyPathfinderCampaignTracker.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<IRecapService, RecapService>();
        services.AddScoped<IChatMessageService, ChatMessageService>();
        services.AddScoped<IGameSessionService, GameSessionService>();
        services.AddScoped<ICampaignNoteService, CampaignNoteService>();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddScoped<ICharacterSheetService, CharacterSheetService>();
        return services;
    }
}
