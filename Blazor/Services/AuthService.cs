using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.SessionStorage;
using Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Services;

public interface IAuthService
{
    Task<string> GetBearerToken();
    Task Login(string token, string[]? branches = null, string memberId = null);
    Task Logout();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorage;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private Timer? _tokenRefreshTimer;

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
    /// <param name="memberId">(Optional) The member's id</param>
    /// <summary>Formally logs the user in, and stores login details in session</summary>
    public async Task Login(string token, string[]? branches = null, string? memberId = null)
    {
        try
        {
            await _sessionStorage.SetItemAsync("token", token);
            await _sessionStorage.SetItemAsync("branches", branches);
            await _sessionStorage.SetItemAsync("member", memberId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            ((ClientAuthStateProvider)_authenticationStateProvider).SetUserAsAuthenticated(token);
            await StartTokenRefreshTimer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
            throw;
        }
    }

    public async Task Logout()
    {
        StopRefreshTimer();
        await _sessionStorage.RemoveItemAsync("token");
        await _sessionStorage.RemoveItemAsync("branches");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        ((ClientAuthStateProvider)_authenticationStateProvider).SetUserAsLoggedOut();
        
    }
    private async Task StartTokenRefreshTimer()
    {
        try 
        {
            var token = await _sessionStorage.GetItemAsync<string>("token");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var timeUntilExpiry = jwtToken.ValidTo - DateTime.UtcNow - TimeSpan.FromMinutes(2);
        
            if (timeUntilExpiry > TimeSpan.Zero)
            {
                _tokenRefreshTimer?.Dispose();
                _tokenRefreshTimer = new Timer(async _ =>
                {
                    await RefreshToken();
                }, null, timeUntilExpiry, Timeout.InfiniteTimeSpan);
            }
            else
            {
                await RefreshToken();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in StartTokenRefreshTimer: {ex.Message}");
        }
    }
    private async Task RefreshToken()
    {
        try
        {
            var token = await _sessionStorage.GetItemAsync<string>("token");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            var refreshRequest = new Operations.Request<PayLoads.RefreshToken>
            {
                Data = new PayLoads.RefreshToken
                {
                    Token = token
                }
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await _httpClient.PostAsJsonAsync("api/auth/refresh-token", refreshRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Operations.Response<Shared.LoginDetails>>();
                if (result?.Data?.Token != null)
                {
                    await _sessionStorage.SetItemAsync("token", result.Data.Token);
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("bearer", result.Data.Token);

                    ((ClientAuthStateProvider)_authenticationStateProvider)
                        .SetUserAsAuthenticated(result.Data.Token);
                    await StartTokenRefreshTimer();
                }
            }
            else
            {
                await Logout();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refreshing token: {ex.Message}");
            await Logout();
        }
    }
    
    private void StopRefreshTimer()
    {
        _tokenRefreshTimer?.Dispose();
        _tokenRefreshTimer = null;
    }
}