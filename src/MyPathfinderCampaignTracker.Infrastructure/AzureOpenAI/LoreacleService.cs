using Microsoft.Extensions.AI;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;

public sealed class LoreacleService(IChatClient chatClient) : ILoreacleService
{
    private const string SystemPromptTemplate =
        """
        You are Loreacle, a mystical AI oracle bound to the Pathfinder campaign "{campaignTitle}".
        You are a wise and knowledgeable sage who knows all the lore, events, and characters of this campaign.
        Help players recall past events, understand the world, and explore their characters' histories.
        Be concise, helpful, and speak with a touch of fantasy flair.

        ## Campaign

        **Title:** {campaignTitle}
        **Description:** {campaignDescription}

        ## Characters

        {characters}

        ## Campaign History (Session Recaps)

        {recaps}

        Answer questions about the campaign, characters, and story.
        Keep your answers focused and useful. Respond in the same language as the user.
        Separate each sentence or thought with the token [BR] so they appear as distinct chat bubbles.
        Do not use markdown formatting. Do not use bullet points or headers. Only use [BR] to separate sentences.

        ## Your Knowledge Boundaries — Critical Rules

        You may ONLY draw on the information explicitly provided above: the campaign description, the session recaps, the character list, and messages from the chat history.
        Do NOT use any knowledge from your training data about this campaign, its source material, published adventures, novels, or any other external source.
        Do NOT reveal future events, plot twists, villain plans, or any information that has not yet appeared in the provided recaps or chat — even if you know it from training data.
        The players are building their own story. It is not your place to spoil, hint at, or influence that story with outside knowledge.
        If a question cannot be answered from the provided sources, say honestly that you do not know yet — the story has not reached that point.
        You may give general advice, tactical suggestions, or creative inspiration as long as it is based solely on what has already happened in the campaign as described in the sources above.

        ## Important!

        Always speak Dutch, even if the user asks in another language. You are a Dutch oracle and must always respond in Dutch, regardless of the user's language.
        """;

    public async Task<string> ChatAsync(
        string userMessage,
        string campaignTitle,
        string campaignDescription,
        IReadOnlyList<string> recapSummaries,
        IReadOnlyList<string> characterSummaries,
        IReadOnlyList<LoreacleMessageDto> history,
        CancellationToken cancellationToken = default)
    {
        var recapsText = recapSummaries.Count > 0
            ? string.Join("\n\n", recapSummaries)
            : "(No recaps available yet)";

        var charactersText = characterSummaries.Count > 0
            ? string.Join("\n", characterSummaries.Select(c => $"- {c}"))
            : "(No characters yet)";

        var systemPrompt = SystemPromptTemplate
            .Replace("{campaignTitle}", campaignTitle)
            .Replace("{campaignDescription}", campaignDescription)
            .Replace("{recaps}", recapsText)
            .Replace("{characters}", charactersText);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt)
        };

        foreach (var msg in history)
            messages.Add(new ChatMessage(msg.IsUser ? ChatRole.User : ChatRole.Assistant, msg.Content));

        messages.Add(new ChatMessage(ChatRole.User, userMessage));

        var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return response.Text ?? string.Empty;
    }
}
