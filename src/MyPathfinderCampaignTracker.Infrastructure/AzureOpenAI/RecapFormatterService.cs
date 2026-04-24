using Microsoft.Extensions.AI;
using MyPathfinderCampaignTracker.Application.Interfaces;

namespace MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;

public sealed class RecapFormatterService(IChatClient chatClient) : IRecapFormatterService
{
    private const string SystemPrompt =
        """
        You are a careful text editor for a Dutch Pathfinder RPG campaign tracker.
        You will receive the raw contents of a session recap. Your task is to improve its formatting and fix spelling errors.

        Rules you MUST follow:
        - Do NOT change, rewrite, summarize, expand, or remove any content or events
        - Add a blank line between paragraphs or alineas where appropriate
        - Remove unwanted line breaks in the middle of sentences (join them into one line)
        - Fix obvious Dutch spelling and grammar mistakes
        - Do NOT translate anything
        - Do NOT add headers, bullet points, or any other structure that was not already there
        - Return ONLY the formatted text, nothing else — no explanations, no preamble
        """;

    public async Task<string> FormatContentsAsync(string contents, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, contents)
        };

        var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return response.Text?.Trim() ?? contents;
    }
}
