using Microsoft.JSInterop;

namespace Blazor.Services;

/// <summary>
/// Service to handle Google reCAPTCHA v3 verification
/// </summary>
public class ReCaptchaService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly bool _isDevelopment;
    
    public ReCaptchaService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        // For development purposes, we'll detect if we're in a development environment
        #if DEBUG
        _isDevelopment = true;
        #else
        _isDevelopment = false;
        #endif
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
            // In development environment, we can return a mock token
            if (_isDevelopment)
            {
                Console.WriteLine("Development environment detected - using mock reCAPTCHA token");
                return "dev_mock_token_12345";
            }
            
            return await _jsRuntime.InvokeAsync<string>("recaptchaExecute", action);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"reCAPTCHA execution failed: {ex.Message}");
            // In development, return a mock token even on failure
            if (_isDevelopment)
            {
                return "dev_mock_token_error_12345";
            }
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
        
        // In development environment, always return true
        if (_isDevelopment)
        {
            Console.WriteLine("Development environment detected - mocking reCAPTCHA verification as successful");
            return true;
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