using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Services;

public class ApiClient(
    IHttpClientFactory httpClientFactory,
    AuthenticationStateProvider authStateProvider,
    AuthStateMonitor authStateMonitor)
{
    private async Task<(HttpClient Client, bool HasToken)> CreateClientAsync()
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var token = authState.User.FindFirst("access_token")?.Value;
        if (token is not null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return (client, true);
        }
        return (client, false);
    }

    /// <summary>
    /// Performs a GET and deserializes the response.
    /// Reports a stale token when the request included a token but got 401; the
    /// <see cref="AuthStateMonitor"/> event is handled in MainLayout where
    /// <c>RendererInfo.IsInteractive</c> reliably gates the navigation.
    /// </summary>
    private async Task<T?> GetJsonAsync<T>(string url)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (hasToken)
                authStateMonitor.ReportStaleToken();
            return default;
        }
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<T>();
    }

    /// <summary>
    /// Checks a write-operation response for 401 and reports a stale token when a token was sent but rejected.
    /// </summary>
    private void HandleUnauthorized(HttpResponseMessage response, bool hasToken)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized && hasToken)
            authStateMonitor.ReportStaleToken();
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await GetJsonAsync<List<UserDto>>("/api/users/") ?? [];
    }

    public async Task<bool> ApproveUserAsync(Guid userId)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PutAsync($"/api/users/{userId}/approve", null);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PromoteUserAsync(Guid userId)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PutAsync($"/api/users/{userId}/promote", null);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/users/{userId}");
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DemoteUserAsync(Guid userId)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PutAsync($"/api/users/{userId}/demote", null);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task SetDarkModeAsync(bool isDarkMode)
    {
        var (client, _) = await CreateClientAsync();
        await client.PutAsJsonAsync("/api/profile/darkmode", new { isDarkMode });
    }

    public async Task<List<CampaignDto>> GetMyCampaignsAsync()
    {
        return await GetJsonAsync<List<CampaignDto>>("/api/campaigns/my") ?? [];
    }

    public async Task<List<CampaignDto>> GetCampaignsAsync()
    {
        return await GetJsonAsync<List<CampaignDto>>("/api/campaigns/") ?? [];
    }

    public async Task<CampaignDto?> GetCampaignAsync(Guid id)
    {
        return await GetJsonAsync<CampaignDto>($"/api/campaigns/{id}");
    }

    public async Task<CampaignDto?> CreateCampaignAsync(CampaignRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync("/api/campaigns/", request);
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CampaignDto>();
    }

    public async Task<bool> UpdateCampaignAsync(Guid id, CampaignRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/campaigns/{id}", request);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCampaignAsync(Guid id)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/campaigns/{id}");
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddCampaignPlayerAsync(Guid campaignId, Guid userId)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsync($"/api/campaigns/{campaignId}/players/{userId}", null);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveCampaignPlayerAsync(Guid campaignId, Guid userId)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/campaigns/{campaignId}/players/{userId}");
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CharacterDto>> GetMyCharactersAsync()
    {
        return await GetJsonAsync<List<CharacterDto>>("/api/characters/my") ?? [];
    }

    public async Task<List<CharacterDto>> GetCharactersAsync(Guid campaignId)
    {
        return await GetJsonAsync<List<CharacterDto>>($"/api/characters/?campaignId={campaignId}") ?? [];
    }

    public async Task<CharacterDto?> GetCharacterAsync(Guid id)
    {
        return await GetJsonAsync<CharacterDto>($"/api/characters/{id}");
    }

    public async Task<CharacterDto?> CreateCharacterAsync(Guid campaignId, CharacterRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/characters/?campaignId={campaignId}", request);
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CharacterDto>();
    }

    public async Task<bool> UpdateCharacterAsync(Guid id, CharacterRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/characters/{id}", request);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCharacterAsync(Guid id)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/characters/{id}");
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UploadCharacterPhotoAsync(Guid id, IBrowserFile file)
    {
        var (client, hasToken) = await CreateClientAsync();
        await using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "photo", file.Name);
        var response = await client.PostAsync($"/api/characters/{id}/photo", content);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<byte[]?> GetCharacterPhotoAsync(Guid id)
    {
        var (client, _) = await CreateClientAsync();
        var response = await client.GetAsync($"/api/characters/{id}/photo");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<List<RecapDto>> GetMyRecapsAsync()
    {
        return await GetJsonAsync<List<RecapDto>>("/api/recaps/my") ?? [];
    }

    public async Task<List<RecapDto>> GetRecapsAsync(Guid campaignId)
    {
        return await GetJsonAsync<List<RecapDto>>($"/api/recaps/?campaignId={campaignId}") ?? [];
    }

    public async Task<RecapDto?> CreateRecapAsync(Guid campaignId, RecapRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/recaps/?campaignId={campaignId}", request);
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<RecapDto>();
    }

    public async Task<bool> UpdateRecapAsync(Guid id, RecapRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/recaps/{id}", request);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteRecapAsync(Guid id)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/recaps/{id}");
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GenerateRecapTitleAsync(string contents)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync("/api/recaps/generate-title", new RecapGenerateTitleRequest(contents));
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<RecapGenerateTitleResponse>();
        return result?.Title;
    }

    public async Task<string?> FormatRecapContentsAsync(string contents)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync("/api/recaps/format-contents", new RecapFormatContentsRequest(contents));
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<RecapFormatContentsResponse>();
        return result?.FormattedContents;
    }

    public async Task<List<LoreacleMessageDto>> GetLoreacleHistoryAsync(Guid campaignId)
    {
        return await GetJsonAsync<List<LoreacleMessageDto>>($"/api/campaigns/{campaignId}/loreacle/history") ?? [];
    }

    public async Task<string?> ChatWithLoreacleAsync(Guid campaignId, LoreacleRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/campaigns/{campaignId}/loreacle", request);
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<LoreacleResponse>();
        return result?.Reply;
    }

    public async Task<List<ChatMessageDto>> GetChatMessagesAsync(Guid campaignId)
    {
        return await GetJsonAsync<List<ChatMessageDto>>($"/api/campaigns/{campaignId}/chat/") ?? [];
    }

    public async Task<ChatMessageDto?> SendChatMessageAsync(Guid campaignId, string content)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/campaigns/{campaignId}/chat/", new ChatMessageRequest(content));
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ChatMessageDto>();
    }

    public async Task<List<GameSessionDto>> GetGameSessionsAsync(Guid campaignId)
    {
        return await GetJsonAsync<List<GameSessionDto>>($"/api/campaigns/{campaignId}/sessions/") ?? [];
    }

    public async Task<GameSessionDto?> GetNextGameSessionAsync(Guid campaignId)
    {
        var (client, _) = await CreateClientAsync();
        var response = await client.GetAsync($"/api/campaigns/{campaignId}/sessions/next");
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return null;
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GameSessionDto>();
    }

    public async Task<GameSessionDto?> CreateGameSessionAsync(Guid campaignId, GameSessionRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/campaigns/{campaignId}/sessions/", request);
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GameSessionDto>();
    }

    public async Task<bool> UpdateGameSessionAsync(Guid campaignId, Guid id, GameSessionRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/campaigns/{campaignId}/sessions/{id}", request);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteGameSessionAsync(Guid campaignId, Guid id)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/campaigns/{campaignId}/sessions/{id}");
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CampaignNoteDto>> GetNotesAsync(Guid campaignId)
    {
        return await GetJsonAsync<List<CampaignNoteDto>>($"/api/campaigns/{campaignId}/notes/") ?? [];
    }

    public async Task<CampaignNoteDto?> CreateNoteAsync(Guid campaignId, CampaignNoteRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/campaigns/{campaignId}/notes/", request);
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CampaignNoteDto>();
    }

    public async Task<bool> UpdateNoteAsync(Guid campaignId, Guid id, CampaignNoteRequest request)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/campaigns/{campaignId}/notes/{id}", request);
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteNoteAsync(Guid campaignId, Guid id)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/campaigns/{campaignId}/notes/{id}");
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateDescriptionAsync(Guid campaignId, string description)
    {
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PatchAsJsonAsync($"/api/campaigns/{campaignId}/description", new DescriptionUpdateRequest(description));
        HandleUnauthorized(response, hasToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> TranslateToNlAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        var (client, hasToken) = await CreateClientAsync();
        var response = await client.PostAsJsonAsync("/api/translate/", new TranslationRequest(text));
        HandleUnauthorized(response, hasToken);
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<TranslationResponse>();
        return result?.TranslatedText;
    }

    public async Task<(Guid Id, string Title)?> GetFavoriteCampaignAsync()
    {
        var result = await GetJsonAsync<FavoriteCampaignResponse>("/api/profile/favorite-campaign");
        if (result?.Id is null) return null;
        return (result.Id.Value, result.Title ?? string.Empty);
    }

    public async Task SetFavoriteCampaignAsync(Guid? campaignId)
    {
        var (client, _) = await CreateClientAsync();
        await client.PutAsJsonAsync("/api/profile/favorite-campaign", new { campaignId });
    }

    private record FavoriteCampaignResponse(Guid? Id, string? Title);
}
