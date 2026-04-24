using Microsoft.Extensions.AI;
using MyPathfinderCampaignTracker.Application.Interfaces;

namespace MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;

public sealed class RecapTitleGeneratorService(IChatClient chatClient) : IRecapTitleGeneratorService
{
    private const string SystemPrompt =
        """
        You are a creative assistant for a Pathfinder RPG campaign tracker.
        Generate a short, evocative Dutch title (maximum 8 words) for a session recap based on the provided text.
        The title should capture the most dramatic or important moment of the session.
        Do NOT include the word "Terugblik", "Recap", or a session number in the title.
        Do NOT use quotation marks. Return ONLY the title, nothing else.
        Always respond in Dutch.
        """;

    public async Task<string> GenerateTitleAsync(string contents, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, contents)
        };

        var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return response.Text?.Trim() ?? string.Empty;
    }
}
