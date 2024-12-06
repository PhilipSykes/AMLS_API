using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Services;

public class AuthStateService : AuthenticationStateProvider
{
    private readonly ISessionStorageService _sessionStorage;

    public AuthStateService(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public event Action? OnChange;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _sessionStorage.GetItemAsync<string>("token");
        
        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
        
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task<string> GetBearerToken()
    {
        return await _sessionStorage.GetItemAsync<string>("token");
    }

    public async Task Login(string token, string userID)
    {
        await _sessionStorage.SetItemAsync("token", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        OnChange?.Invoke();
    }

    public async Task Logout()
    {
        await _sessionStorage.RemoveItemAsync("token");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        OnChange?.Invoke();
    }
}