using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using MyPathfinderCampaignTracker.Application.Models;

namespace MyPathfinderCampaignTracker.Web.Services;

public class ApiClient(
    IHttpClientFactory httpClientFactory,
    AuthenticationStateProvider authStateProvider)
{
    private async Task<HttpClient> CreateClientAsync()
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var token = authState.User.FindFirst("access_token")?.Value;
        if (token is not null)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var client = await CreateClientAsync();
        var users = await client.GetFromJsonAsync<List<UserDto>>("/api/users/");
        return users ?? [];
    }

    public async Task<bool> ApproveUserAsync(Guid userId)
    {
        var client = await CreateClientAsync();
        var response = await client.PutAsync($"/api/users/{userId}/approve", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PromoteUserAsync(Guid userId)
    {
        var client = await CreateClientAsync();
        var response = await client.PutAsync($"/api/users/{userId}/promote", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DemoteUserAsync(Guid userId)
    {
        var client = await CreateClientAsync();
        var response = await client.PutAsync($"/api/users/{userId}/demote", null);
        return response.IsSuccessStatusCode;
    }

    public async Task SetDarkModeAsync(bool isDarkMode)
    {
        var client = await CreateClientAsync();
        await client.PutAsJsonAsync("/api/profile/darkmode", new { isDarkMode });
    }

    public async Task<List<CampaignDto>> GetMyCampaignsAsync()
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<List<CampaignDto>>("/api/campaigns/my") ?? [];
    }

    public async Task<List<CampaignDto>> GetCampaignsAsync()
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<List<CampaignDto>>("/api/campaigns/") ?? [];
    }

    public async Task<CampaignDto?> GetCampaignAsync(Guid id)
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<CampaignDto>($"/api/campaigns/{id}");
    }

    public async Task<CampaignDto?> CreateCampaignAsync(CampaignRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync("/api/campaigns/", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CampaignDto>();
    }

    public async Task<bool> UpdateCampaignAsync(Guid id, CampaignRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/campaigns/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCampaignAsync(Guid id)
    {
        var client = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/campaigns/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddCampaignPlayerAsync(Guid campaignId, Guid userId)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsync($"/api/campaigns/{campaignId}/players/{userId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveCampaignPlayerAsync(Guid campaignId, Guid userId)
    {
        var client = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/campaigns/{campaignId}/players/{userId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CharacterDto>> GetMyCharactersAsync()
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<List<CharacterDto>>("/api/characters/my") ?? [];
    }

    public async Task<List<CharacterDto>> GetCharactersAsync(Guid campaignId)
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<List<CharacterDto>>($"/api/characters/?campaignId={campaignId}") ?? [];
    }

    public async Task<CharacterDto?> GetCharacterAsync(Guid id)
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<CharacterDto>($"/api/characters/{id}");
    }

    public async Task<CharacterDto?> CreateCharacterAsync(Guid campaignId, CharacterRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/characters/?campaignId={campaignId}", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CharacterDto>();
    }

    public async Task<bool> UpdateCharacterAsync(Guid id, CharacterRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/characters/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCharacterAsync(Guid id)
    {
        var client = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/characters/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<RecapDto>> GetMyRecapsAsync()
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<List<RecapDto>>("/api/recaps/my") ?? [];
    }

    public async Task<List<RecapDto>> GetRecapsAsync(Guid campaignId)
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<List<RecapDto>>($"/api/recaps/?campaignId={campaignId}") ?? [];
    }

    public async Task<RecapDto?> CreateRecapAsync(Guid campaignId, RecapRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/recaps/?campaignId={campaignId}", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<RecapDto>();
    }

    public async Task<bool> UpdateRecapAsync(Guid id, RecapRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/recaps/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteRecapAsync(Guid id)
    {
        var client = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/recaps/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GenerateRecapTitleAsync(string contents)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync("/api/recaps/generate-title", new RecapGenerateTitleRequest(contents));
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<RecapGenerateTitleResponse>();
        return result?.Title;
    }

    public async Task<string?> FormatRecapContentsAsync(string contents)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync("/api/recaps/format-contents", new RecapFormatContentsRequest(contents));
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<RecapFormatContentsResponse>();
        return result?.FormattedContents;
    }

    public async Task<string?> ChatWithLoreacleAsync(Guid campaignId, LoreacleRequest request)    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/campaigns/{campaignId}/loreacle", request);
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<LoreacleResponse>();
        return result?.Reply;
    }

    public async Task<List<ChatMessageDto>> GetChatMessagesAsync(Guid campaignId)
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<List<ChatMessageDto>>($"/api/campaigns/{campaignId}/chat/") ?? [];
    }

    public async Task<ChatMessageDto?> SendChatMessageAsync(Guid campaignId, string content)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/campaigns/{campaignId}/chat/", new ChatMessageRequest(content));
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ChatMessageDto>();
    }

    public async Task<List<GameSessionDto>> GetGameSessionsAsync(Guid campaignId)
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<List<GameSessionDto>>($"/api/campaigns/{campaignId}/sessions/") ?? [];
    }

    public async Task<GameSessionDto?> GetNextGameSessionAsync(Guid campaignId)
    {
        var client = await CreateClientAsync();
        var response = await client.GetAsync($"/api/campaigns/{campaignId}/sessions/next");
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return null;
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GameSessionDto>();
    }

    public async Task<GameSessionDto?> CreateGameSessionAsync(Guid campaignId, GameSessionRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/campaigns/{campaignId}/sessions/", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GameSessionDto>();
    }

    public async Task<bool> UpdateGameSessionAsync(Guid campaignId, Guid id, GameSessionRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/campaigns/{campaignId}/sessions/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteGameSessionAsync(Guid campaignId, Guid id)
    {
        var client = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/campaigns/{campaignId}/sessions/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CampaignNoteDto>> GetNotesAsync(Guid campaignId)
    {
        var client = await CreateClientAsync();
        return await client.GetFromJsonAsync<List<CampaignNoteDto>>($"/api/campaigns/{campaignId}/notes/") ?? [];
    }

    public async Task<CampaignNoteDto?> CreateNoteAsync(Guid campaignId, CampaignNoteRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync($"/api/campaigns/{campaignId}/notes/", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CampaignNoteDto>();
    }

    public async Task<bool> UpdateNoteAsync(Guid campaignId, Guid id, CampaignNoteRequest request)
    {
        var client = await CreateClientAsync();
        var response = await client.PutAsJsonAsync($"/api/campaigns/{campaignId}/notes/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteNoteAsync(Guid campaignId, Guid id)
    {
        var client = await CreateClientAsync();
        var response = await client.DeleteAsync($"/api/campaigns/{campaignId}/notes/{id}");
        return response.IsSuccessStatusCode;
    }
}
