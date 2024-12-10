using System.Net.Http.Headers;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Services;

public interface IAuthService
{
    Task<string> GetBearerToken();
    Task Login(string token);
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

    public async Task Login(string token)
    {
        await _sessionStorage.SetItemAsync("token", token);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        ((ClientAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(token);
    }

    public async Task Logout()
    {
        await _sessionStorage.RemoveItemAsync("token");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        ((ClientAuthStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
    }
}