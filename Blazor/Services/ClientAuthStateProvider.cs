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
    /// <summary>
    /// Retrieves the current authentication state of the user
    /// </summary>
    /// <returns>
    /// AuthenticationState containing the user's claims principal.
    /// Returns an anonymous user if no valid token exists.
    /// </returns>
    /// <remarks>
    /// This method:
    /// - Checks for a valid token in session storage
    /// - Sets up HTTP authentication headers if token exists
    /// - Extracts user claims from the JWT token
    /// </remarks>
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
    /// <summary>
    /// Updates the authentication state when a user successfully logs in
    /// </summary>
    /// <param name="token">JWT token containing the user's claims</param>
    /// <remarks>
    /// Extracts claims from the JWT token and notifies the application
    /// of the authentication state change
    /// </remarks>
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