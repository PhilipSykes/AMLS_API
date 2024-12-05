using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.SessionStorage;

namespace Blazor.Services;

public class AuthStateService
{
    private readonly ISessionStorageService _sessionStorage;

    public AuthStateService(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public event Action? OnChange;

    public async Task<ClaimsPrincipal?> GetUser()
    {
        var token = await _sessionStorage.GetItemAsync<string>("token");
        if (string.IsNullOrEmpty(token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
    }

    public async Task<string> GetBearerToken()
    {
        var token = await _sessionStorage.GetItemAsync<string>("token");
        if (string.IsNullOrEmpty(token))
            return null;
        return token;
    }

    public async Task Login(string token, string username)
    {
        await _sessionStorage.SetItemAsync("token", token);
        NotifyStateChanged();
    }

    public async Task Logout()
    {
        await _sessionStorage.RemoveItemAsync("token");
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}