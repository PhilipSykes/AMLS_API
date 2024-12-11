using System.Net.Http.Headers;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Services;

public interface IAuthService
{
    Task<string> GetBearerToken();
    Task Login(string token, string[]? branches = null);
    Task Logout();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorage;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(ISessionStorageService sessionStorage, HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _sessionStorage = sessionStorage;
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
    }
    
    public async Task<string> GetBearerToken()
    {
        return await _sessionStorage.GetItemAsync<string>("token");
    }

    /// <summary>
    /// Logs a user in
    /// </summary>
    /// <param name="token">Token for authorisation</param>
    /// <param name="branches">(Optional) Array of branch codes for this user</param>
    /// <summary>Formally logs the user in, and stores login details in session</summary>
    public async Task Login(string token, string[]? branches = null)
    {
        await _sessionStorage.SetItemAsync("token", token);
        await _sessionStorage.SetItemAsync("branches", branches);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        ((ClientAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(token);
    }

    public async Task Logout()
    {
        await _sessionStorage.RemoveItemAsync("token");
        await _sessionStorage.RemoveItemAsync("branches");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        ((ClientAuthStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
    }
}