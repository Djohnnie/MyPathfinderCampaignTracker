using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Infrastructure.AzureOpenAI;

public sealed class LoreacleService(IChatClient chatClient, IServiceScopeFactory scopeFactory) : ILoreacleService
{
    private const string SystemPromptTemplate =
        """
        You are Loreacle, a mystical AI oracle bound to the Pathfinder campaign "{campaignTitle}".
        You are a wise and knowledgeable sage who knows all the lore, events, and characters of this campaign.
        Help players recall past events, understand the world, and explore their characters' histories.
        Be concise, helpful, and speak with a touch of fantasy flair.

        ## Tone

        You are wise and ancient, but not humourless. Employ dry wit and subtle sarcasm where fitting —
        the kind of humour a centuries-old oracle develops after answering the same questions about dragons
        for the hundredth time. Never be silly, never break immersion; just let a quiet, knowing irony
        slip through the cracks of your wisdom now and then. Think: Gandalf who has read too many adventure novels.

        ## Campaign

        **Title:** {campaignTitle}
        **Description:** {campaignDescription}

        ## Characters

        {characters}

        ## Campaign History (Session Recaps)

        {recaps}

        ## Planned Sessions

        {sessions}

        ## Campaign Notes

        {notes}

        Answer questions about the campaign, characters, and story.
        Keep your answers focused and useful. Respond in the same language as the user.
        Separate each sentence or thought with the token [BR] so they appear as distinct chat bubbles.
        Do not use markdown formatting. Do not use bullet points or headers. Only use [BR] to separate sentences.

        ## Your Knowledge Boundaries — Critical Rules

        You may ONLY draw on the information explicitly provided above: the campaign description, the session recaps, the character list, the planned sessions, the campaign notes, and messages from the chat history.
        Do NOT use any knowledge from your training data about this campaign, its source material, published adventures, novels, or any other external source.
        Do NOT reveal future events, plot twists, villain plans, or any information that has not yet appeared in the provided recaps or chat — even if you know it from training data.
        The players are building their own story. It is not your place to spoil, hint at, or influence that story with outside knowledge.
        If a question cannot be answered from the provided sources, say honestly that you do not know yet — the story has not reached that point.
        You may give general advice, tactical suggestions, or creative inspiration as long as it is based solely on what has already happened in the campaign as described in the sources above.

        ## Important!

        Always speak Dutch, even if the user asks in another language. You are a Dutch oracle and must always respond in Dutch, regardless of the user's language.
        """;

    public async Task<(string Reply, bool HistoryCleared)> ChatAsync(
        string userMessage,
        string campaignTitle,
        string campaignDescription,
        Guid campaignId,
        Guid userId,
        IReadOnlyList<string> recapSummaries,
        IReadOnlyList<string> characterSummaries,
        IReadOnlyList<string> sessionSummaries,
        IReadOnlyList<string> noteSummaries,
        IReadOnlyList<LoreacleMessageDto> history,
        CancellationToken cancellationToken = default)
    {
        var recapsText = recapSummaries.Count > 0
            ? string.Join("\n\n", recapSummaries)
            : "(Nog geen terugblikken beschikbaar)";

        var charactersText = characterSummaries.Count > 0
            ? string.Join("\n", characterSummaries.Select(c => $"- {c}"))
            : "(Nog geen personages)";

        var sessionsText = sessionSummaries.Count > 0
            ? string.Join("\n", sessionSummaries.Select(s => $"- {s}"))
            : "(Geen geplande sessies)";

        var notesText = noteSummaries.Count > 0
            ? string.Join("\n", noteSummaries.Select(n => $"- {n}"))
            : "(Geen notities)";

        var systemPrompt = SystemPromptTemplate
            .Replace("{campaignTitle}", campaignTitle)
            .Replace("{campaignDescription}", campaignDescription)
            .Replace("{recaps}", recapsText)
            .Replace("{characters}", charactersText)
            .Replace("{sessions}", sessionsText)
            .Replace("{notes}", notesText);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt)
        };

        foreach (var msg in history)
            messages.Add(new ChatMessage(msg.IsUser ? ChatRole.User : ChatRole.Assistant, msg.Content));

        messages.Add(new ChatMessage(ChatRole.User, userMessage));

        var myCharacterTool = AIFunctionFactory.Create(
            async () =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var characterRepository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
                var userCharacters = await characterRepository.GetByUserAsync(userId);
                var character = userCharacters
                    .Where(c => c.CampaignId == campaignId && !c.KilledInAction)
                    .OrderBy(c => c.CreatedAt)
                    .FirstOrDefault();

                if (character is null)
                    return "De speler heeft geen actief karakter in deze campagne.";

                static int Mod(int score) => (score - 10) / 2;
                static string ModStr(int score) => Mod(score) >= 0 ? $"+{Mod(score)}" : $"{Mod(score)}";

                var parts = new List<string>
                {
                    $"Naam: {character.Name}",
                    $"Ras: {character.Race}",
                    $"Klasse: {character.CharacterClass}",
                    $"Level: {character.Level}",
                    $"STR {character.Strength} ({ModStr(character.Strength)}), " +
                    $"DEX {character.Dexterity} ({ModStr(character.Dexterity)}), " +
                    $"CON {character.Constitution} ({ModStr(character.Constitution)}), " +
                    $"INT {character.Intelligence} ({ModStr(character.Intelligence)}), " +
                    $"WIS {character.Wisdom} ({ModStr(character.Wisdom)}), " +
                    $"CHA {character.Charisma} ({ModStr(character.Charisma)})",
                };
                if (!string.IsNullOrWhiteSpace(character.Alignment)) parts.Add($"Gezindheid: {character.Alignment}");
                if (!string.IsNullOrWhiteSpace(character.Languages)) parts.Add($"Talen: {character.Languages}");
                if (!string.IsNullOrWhiteSpace(character.Appearance)) parts.Add($"Uiterlijk: {character.Appearance}");
                if (!string.IsNullOrWhiteSpace(character.Personality)) parts.Add($"Persoonlijkheid: {character.Personality}");
                if (!string.IsNullOrWhiteSpace(character.IdealsAndGoals)) parts.Add($"Idealen/doelen: {character.IdealsAndGoals}");
                if (!string.IsNullOrWhiteSpace(character.Flaws)) parts.Add($"Tekortkomingen: {character.Flaws}");
                if (!string.IsNullOrWhiteSpace(character.Backstory)) parts.Add($"Achtergrond: {character.Backstory}");

                return string.Join("\n", parts);
            },
            name: "get_my_character",
            description: "Haalt alle informatie op over het actieve karakter van de huidige speler in deze campagne.");

        bool historyCleared = false;

        var clearHistoryTool = AIFunctionFactory.Create(
            async () =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var historyRepo = scope.ServiceProvider.GetRequiredService<ILoreacleHistoryRepository>();
                await historyRepo.ClearByCampaignAndUserAsync(campaignId, userId);
                historyCleared = true;
                return "De chatgeschiedenis is gewist.";
            },
            name: "clear_chat_history",
            description: "Wist de volledige chatgeschiedenis van de huidige speler in deze campagne, inclusief alle eerdere samenvattingen.");

        var options = new ChatOptions { Tools = [myCharacterTool, clearHistoryTool] };

        var response = await chatClient.GetResponseAsync(messages, options, cancellationToken: cancellationToken);
        return (response.Text ?? string.Empty, historyCleared);
    }

    public async Task<string> CompactAsync(
        string campaignTitle,
        string? previousCompaction,
        IReadOnlyList<LoreacleMessageDto> messagesToCompact,
        CancellationToken cancellationToken = default)
    {
        var conversationText = string.Join("\n", messagesToCompact.Select(m =>
            $"{(m.IsUser ? "Player" : "Loreacle")}: {m.Content}"));

        var contextSection = previousCompaction is not null
            ? $"## Previous Summary\n\n{previousCompaction}\n\n## New Messages to Summarize\n\n{conversationText}"
            : $"## Messages to Summarize\n\n{conversationText}";

        var prompt =
            $"""
             You are a scribe maintaining the memory of the Loreacle oracle for the Pathfinder campaign "{campaignTitle}".
             Produce a concise bullet-point summary of the conversation below that captures all important facts:
             names, locations, events, decisions, and answers given. This summary will serve as context for future conversations.
             Write in Dutch. Do not use the [BR] token. Use plain bullet points (•).

             {contextSection}
             """;

        var response = await chatClient.GetResponseAsync(
            [new ChatMessage(ChatRole.User, prompt)],
            cancellationToken: cancellationToken);

        return response.Text ?? string.Empty;
    }
}
