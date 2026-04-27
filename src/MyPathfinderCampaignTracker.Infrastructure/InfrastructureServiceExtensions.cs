using Azure.AI.OpenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;
using MyPathfinderCampaignTracker.Infrastructure.Data;
using MyPathfinderCampaignTracker.Infrastructure.Repositories;
using MyPathfinderCampaignTracker.Infrastructure.Services;
using System.ClientModel;

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
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<IRecapRepository, RecapRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IGameSessionRepository, GameSessionRepository>();
        services.AddScoped<ICampaignNoteRepository, CampaignNoteRepository>();
        services.AddScoped<ILoreacleHistoryRepository, LoreacleHistoryRepository>();
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
        services.AddScoped<ICharacterSheetRepository, CharacterSheetRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IImageProcessingService, ImageProcessingService>();

        var endpoint = configuration["AZURE_OPENAI_ENDPOINT"];
        var apiKey = configuration["AZURE_OPENAI_KEY"];
        var model = configuration["AZURE_OPENAI_MODEL"] ?? "gpt-4o";

        if (!string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(apiKey))
        {
            services.AddSingleton<IChatClient>(_ =>
            {
                var innerClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
                    .GetChatClient(model)
                    .AsIChatClient();
                return new FunctionInvokingChatClient(innerClient);
            });
            services.AddSingleton<ILoreacleService, LoreacleService>();
            services.AddSingleton<IRecapTitleGeneratorService, RecapTitleGeneratorService>();
            services.AddSingleton<IRecapFormatterService, RecapFormatterService>();
            services.AddSingleton<ITranslationService, TranslationService>();
        }
        else
        {
            services.AddSingleton<ILoreacleService, NoOpLoreacleService>();
            services.AddSingleton<IRecapTitleGeneratorService, NoOpRecapTitleGeneratorService>();
            services.AddSingleton<IRecapFormatterService, NoOpRecapFormatterService>();
            services.AddSingleton<ITranslationService, NoOpTranslationService>();
        }

        return services;
    }
}
