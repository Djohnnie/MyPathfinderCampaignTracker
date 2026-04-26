using MyPathfinderCampaignTracker.Application.Interfaces;
using MyPathfinderCampaignTracker.Application.Models;
using MyPathfinderCampaignTracker.Domain.Entities;

namespace MyPathfinderCampaignTracker.Web.Api;

public static class LoreacleEndpoints
{
    private const int CompactionThreshold = 10;

    private static readonly TimeZoneInfo BrusselsZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Brussels");

    private static DateTime ToBrussels(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), BrusselsZone);

    public static IEndpointRouteBuilder MapLoreacleEndpoints(this IEndpointRouteBuilder routes)
    {
        // GET history — only original messages (no compaction rows)
        routes.MapGet("/api/campaigns/{campaignId:guid}/loreacle/history",
            async (Guid campaignId, ILoreacleHistoryRepository historyRepo) =>
            {
                var messages = await historyRepo.GetByCampaignAsync(campaignId);
                var dtos = messages
                    .Where(m => !m.IsCompaction)
                    .Select(m => new LoreacleMessageDto(m.IsUser, m.Content))
                    .ToList();
                return Results.Ok(dtos);
            })
            .RequireAuthorization("ApiAuth");

        routes.MapPost("/api/campaigns/{campaignId:guid}/loreacle",
            async (
                Guid campaignId,
                LoreacleRequest request,
                ICampaignService campaignService,
                ICharacterService characterService,
                IRecapService recapService,
                IGameSessionService gameSessionService,
                ICampaignNoteService campaignNoteService,
                ILoreacleService loreacleService,
                ILoreacleHistoryRepository historyRepo,
                CancellationToken ct) =>
            {
                var campaign = await campaignService.GetByIdAsync(campaignId);
                if (campaign is null)
                    return Results.NotFound();

                var characters = await characterService.GetByCampaignAsync(campaignId);
                var recaps = await recapService.GetByCampaignAsync(campaignId);
                var sessions = await gameSessionService.GetByCampaignAsync(campaignId);
                var notes = await campaignNoteService.GetByCampaignAsync(campaignId);

                var recapSummaries = recaps
                    .OrderByDescending(r => r.Number)
                    .Take(10)
                    .Select(r => $"Recap {r.Number} – {r.Title} ({r.Date:yyyy-MM-dd}):\n{r.Contents}")
                    .ToList();

                static int Mod(int score) => (score - 10) / 2;
                static string ModStr(int score) => Mod(score) >= 0 ? $"+{Mod(score)}" : $"{Mod(score)}";

                var characterSummaries = characters
                    .Select(c =>
                    {
                        var kia = c.KilledInAction ? " †" : "";
                        var scores = $"STR {c.Strength} ({ModStr(c.Strength)}), " +
                                     $"DEX {c.Dexterity} ({ModStr(c.Dexterity)}), " +
                                     $"CON {c.Constitution} ({ModStr(c.Constitution)}), " +
                                     $"INT {c.Intelligence} ({ModStr(c.Intelligence)}), " +
                                     $"WIS {c.Wisdom} ({ModStr(c.Wisdom)}), " +
                                     $"CHA {c.Charisma} ({ModStr(c.Charisma)})";
                        var storyParts = new List<string>();
                        if (!string.IsNullOrWhiteSpace(c.Alignment)) storyParts.Add($"Gezindheid: {c.Alignment}");
                        if (!string.IsNullOrWhiteSpace(c.Languages)) storyParts.Add($"Talen: {c.Languages}");
                        if (!string.IsNullOrWhiteSpace(c.Appearance)) storyParts.Add($"Uiterlijk: {c.Appearance}");
                        if (!string.IsNullOrWhiteSpace(c.Personality)) storyParts.Add($"Persoonlijkheid: {c.Personality}");
                        if (!string.IsNullOrWhiteSpace(c.IdealsAndGoals)) storyParts.Add($"Idealen/doelen: {c.IdealsAndGoals}");
                        if (!string.IsNullOrWhiteSpace(c.Flaws)) storyParts.Add($"Tekortkomingen: {c.Flaws}");
                        var storySuffix = storyParts.Count > 0 ? "\n    " + string.Join(" | ", storyParts) : "";
                        return $"{c.Name} ({c.Race} {c.CharacterClass}, Level {c.Level}){kia} — {scores}{storySuffix}";
                    })
                    .ToList();

                var now = DateTime.UtcNow;
                var sessionSummaries = sessions
                    .OrderBy(s => s.ScheduledAt)
                    .Select(s =>
                    {
                        var local = ToBrussels(s.ScheduledAt);
                        var label = s.ScheduledAt < now ? "gespeeld" : "gepland";
                        return $"{local:yyyy-MM-dd HH:mm} @ {s.Location} ({label})";
                    })
                    .ToList();

                var noteSummaries = notes
                    .Select(n => $"{n.AuthorUsername}: {n.Content}")
                    .ToList();

                // Build AI context: last compaction + all non-compaction messages after it
                var allMessages = await historyRepo.GetByCampaignAsync(campaignId);
                var history = BuildContext(allMessages);

                var reply = await loreacleService.ChatAsync(
                    request.UserMessage,
                    campaign.Title,
                    campaign.Description,
                    recapSummaries,
                    characterSummaries,
                    sessionSummaries,
                    noteSummaries,
                    history,
                    ct);

                // Persist user message and reply
                var sentAt = DateTime.UtcNow;
                await historyRepo.AddAsync(new LoreacleMessage
                {
                    Id = Guid.NewGuid(),
                    CampaignId = campaignId,
                    IsUser = true,
                    Content = request.UserMessage,
                    SentAt = sentAt
                });
                await historyRepo.AddAsync(new LoreacleMessage
                {
                    Id = Guid.NewGuid(),
                    CampaignId = campaignId,
                    IsUser = false,
                    Content = reply,
                    SentAt = sentAt.AddTicks(1)
                });

                // Trigger compaction if enough uncompacted messages have accumulated
                await TryCompactAsync(campaignId, campaign.Title, historyRepo, loreacleService, ct);

                return Results.Ok(new LoreacleResponse(reply));
            })
            .RequireAuthorization("ApiAuth");

        return routes;
    }

    private static IReadOnlyList<LoreacleMessageDto> BuildContext(IReadOnlyList<LoreacleMessage> all)
    {
        // Find the most recent compaction row
        var lastCompaction = all.LastOrDefault(m => m.IsCompaction);

        IEnumerable<LoreacleMessage> context;
        if (lastCompaction is not null)
        {
            // Include the compaction summary itself, then all subsequent non-compaction messages
            context = all.Where(m => m.IsCompaction
                ? m.Id == lastCompaction.Id
                : m.SentAt > lastCompaction.SentAt && !m.IsCompaction);
        }
        else
        {
            context = all.Where(m => !m.IsCompaction);
        }

        return context
            .Select(m => new LoreacleMessageDto(m.IsUser, m.Content))
            .ToList();
    }

    private static async Task TryCompactAsync(
        Guid campaignId,
        string campaignTitle,
        ILoreacleHistoryRepository historyRepo,
        ILoreacleService loreacleService,
        CancellationToken ct)
    {
        var allMessages = await historyRepo.GetByCampaignAsync(campaignId);

        var uncompacted = allMessages
            .Where(m => !m.IsCompaction && !m.IsCompacted)
            .ToList();

        if (uncompacted.Count < CompactionThreshold)
            return;

        var previousCompaction = allMessages.LastOrDefault(m => m.IsCompaction);

        var toCompact = uncompacted.Select(m => new LoreacleMessageDto(m.IsUser, m.Content)).ToList();

        var summary = await loreacleService.CompactAsync(
            campaignTitle,
            previousCompaction?.Content,
            toCompact,
            ct);

        // Mark original messages as compacted
        await historyRepo.MarkAsCompactedAsync(uncompacted.Select(m => m.Id));

        // Save the compaction summary row (IsUser=false, IsCompaction=true)
        await historyRepo.AddAsync(new LoreacleMessage
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            IsUser = false,
            IsCompaction = true,
            Content = summary,
            SentAt = DateTime.UtcNow
        });
    }
}
