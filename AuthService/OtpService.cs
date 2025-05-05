using System.Security.Cryptography;
using Common.Models;
using static Common.Models.Entities;

namespace AuthService;

/// <summary>
/// Service for managing one-time verification codes
/// </summary>
public class OtpService
{
    // In-memory store of active OTP codes (email -> (code, expiration, login object))
    // In a production app, this should use a distributed cache like Redis
    private readonly Dictionary<string, (string Code, DateTime Expiration, Login UserData)> _otpStore = new();
    private const int OtpExpirationMinutes = 10;
    
    /// <summary>
    /// Generates a new 6-digit OTP code for a user and stores it
    /// </summary>
    /// <param name="user">User login data</param>
    /// <returns>The generated OTP code</returns>
    public string GenerateOtp(Login user)
    {
        // Generate a random 6-digit code
        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        
        // Store the code with expiration time
        _otpStore[user.Email] = (code, DateTime.UtcNow.AddMinutes(OtpExpirationMinutes), user);
        
        return code;
    }
    
    /// <summary>
    /// Verifies if a given OTP code is valid for the specified email
    /// </summary>
    /// <param name="email">User's email</param>
    /// <param name="code">OTP code to verify</param>
    /// <returns>The user data if verification succeeded, null otherwise</returns>
    public Login? VerifyOtp(string email, string code)
    {
        // Check if there's an active OTP for this email
        if (!_otpStore.TryGetValue(email, out var otpData))
        {
            return null;
        }
        
        // Check if the OTP has expired
        if (DateTime.UtcNow > otpData.Expiration)
        {
            _otpStore.Remove(email);
            return null;
        }
        
        // Check if the provided code matches
        if (otpData.Code != code)
        {
            return null;
        }
        
        // Code is valid, remove it from the store and return the user data
        _otpStore.Remove(email);
        return otpData.UserData;
    }
} 