using Microsoft.JSInterop;

namespace Blazor.Services;

/// <summary>
/// Service to handle Google reCAPTCHA v3 verification
/// </summary>
public class ReCaptchaService
{
    private readonly IJSRuntime _jsRuntime;
    
    public ReCaptchaService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    /// <summary>
    /// Execute reCAPTCHA verification for a specific action
    /// </summary>
    /// <param name="action">The action being performed (e.g., "login", "register")</param>
    /// <returns>The reCAPTCHA token if successful</returns>
    public async Task<string> ExecuteReCaptchaAsync(string action)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("recaptchaExecute", action);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"reCAPTCHA execution failed: {ex.Message}");
            return string.Empty;
        }
    }
    
    /// <summary>
    /// Verify if a reCAPTCHA token is valid
    /// </summary>
    /// <param name="token">The token to verify</param>
    /// <returns>True if the token is valid, false otherwise</returns>
    public async Task<bool> VerifyTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("verifyRecaptchaToken", token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"reCAPTCHA verification failed: {ex.Message}");
            return false;
        }
    }
} 