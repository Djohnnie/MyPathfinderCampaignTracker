using Microsoft.Extensions.AI;
using MyPathfinderCampaignTracker.Application.Interfaces;

namespace MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;

public sealed class TranslationService(IChatClient chatClient) : ITranslationService
{
    private const string SystemPrompt =
        """
        You are a professional Dutch translator.
        Translate the provided text to Dutch (Netherlands).
        Return ONLY the translated text, nothing else.
        Preserve formatting, line breaks, and punctuation.
        Do NOT add any explanation, notes, or quotation marks.
        """;

    public async Task<string?> TranslateToNlAsync(string text, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, text)
        };

        var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return response.Text?.Trim();
    }
}
