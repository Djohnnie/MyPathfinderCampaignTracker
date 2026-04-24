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
}
