using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.SessionStorage;
using Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Services;

public class ClientAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorage;
    private readonly NavigationManager _navigationManager;
    
    
    public ClientAuthStateProvider(ISessionStorageService sessionStorage, HttpClient httpClient, NavigationManager navigationManager)
    {
        _sessionStorage = sessionStorage;
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _sessionStorage.GetItemAsync<string>("token");

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
        
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
    
    public void SetUserAsAuthenticated(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void SetUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
        _navigationManager.NavigateTo("/login");
    }
}