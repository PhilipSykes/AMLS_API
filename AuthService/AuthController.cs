using Common.MessageBroker;
using Common.Constants;
using Common.Models;
using static Common.Models.Shared;
using static Common.Models.Operations;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Common.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using static Common.Models.Entities;
using System.Net.Http;
using System.Text.Json;

namespace AuthService;

/// <summary>
/// Controller for handling authentication operations
/// </summary>
[ApiController]
[Route("[controller]")] 
public class AuthController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly ISearchRepository<Login> _authSearchRepo;
    private readonly TokenAuthService _tokenAuthService;
    private readonly OtpService _otpService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    
    /// <summary>
    /// Initializes a new instance of the AuthController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="authSearchRepo">Service for user search operations</param>
    /// <param name="tokenAuthService">Service for JWT token operations</param>
    /// <param name="otpService">Service for OTP code management</param>
    /// <param name="httpClientFactory">HTTP client factory for external APIs</param>
    /// <param name="config">Application configuration</param>
    public AuthController(
        Exchange exchange, 
        ISearchRepository<Login> authSearchRepo, 
        TokenAuthService tokenAuthService,
        OtpService otpService,
        IHttpClientFactory httpClientFactory,
        IConfiguration config)
    {
        _exchange = exchange;
        _authSearchRepo = authSearchRepo;
        _tokenAuthService = tokenAuthService;
        _otpService = otpService;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials containing email and password</param>
    /// <returns>Response containing login details and JWT token if successful</returns>
    [HttpPost("login")]
    public async Task<ActionResult<Response<LoginDetails>>> Login([FromBody] Request<PayLoads.Login> request)
    {
        // Verify reCAPTCHA token if present
        if (request.ReCaptchaVerification != null && 
            !string.IsNullOrEmpty(request.ReCaptchaVerification.Token) &&
            !await VerifyReCaptchaTokenAsync(request.ReCaptchaVerification.Token, request.ReCaptchaVerification.Action))
        {
            return BadRequest(new Response<LoginDetails>
            {
                Success = false,
                StatusCode = QueryResultCode.Unauthorized,
                Message = "reCAPTCHA verification failed",
            });
        }
        
        var emailFilter = new List<Filter>
        {
            new Filter(DbFieldNames.Login.Email, request.Data.Email, DbEnums.Equals)
        };
        List<Login> result = await _authSearchRepo.Search(DocumentTypes.Login, emailFilter);

        if (!result.Any())
        {
            return Unauthorized(new Response<LoginDetails>
            {
                Success = false,
                StatusCode = QueryResultCode.BadRequest,
                Message = "No Results Found",
            });
        }
        
        if (!PasswordService.VerifyPassword(result[0].PasswordHash, request.Data.Password))
        {
            return Unauthorized(new Response<LoginDetails>
            {
                Success = false,
                StatusCode = QueryResultCode.Unauthorized,
                Message = "Invalid Credentials",
            });
        }
        
        string token = _tokenAuthService.GenerateJwtToken(result[0]);
        
        //Runs publish message in background
        _ = _exchange.PublishNotification(
            MessageTypes.EmailNotifications.Login, 
            request.EmailDetails);
            
        return Ok(new Response<LoginDetails>
        {
            Success = true,
            Message = "Login successful",
            StatusCode = QueryResultCode.Ok,
            Data = new LoginDetails()
            {
                UserID = result[0].UserID,
                Branches = result[0].Branches,
                Token = token
            }
        });
    }
    
    /// <summary>
    /// Verifies the OTP code and returns a JWT token if valid
    /// </summary>
    /// <param name="request">Verification code and email</param>
    /// <returns>Response containing JWT token if code is valid</returns>
    [HttpPost("verify-code")]
    public ActionResult<Response<LoginDetails>> VerifyCode([FromBody] Request<PayLoads.VerifyCode> request)
    {
        var user = _otpService.VerifyOtp(request.Data.Email, request.Data.Code);
        
        if (user == null)
        {
            return Unauthorized(new Response<LoginDetails>
            {
                Success = false,
                StatusCode = QueryResultCode.Unauthorized,
                Message = "Invalid or expired code",
            });
        }
        
        string token = _tokenAuthService.GenerateJwtToken(user);
        
        // Send login confirmation email
        if (request.EmailDetails != null)
        {
            _ = _exchange.PublishNotification(
                MessageTypes.EmailNotifications.Login, 
                request.EmailDetails);
        }
        
        return Ok(new Response<LoginDetails>
        {
            Success = true,
            Message = "Login successful",
            StatusCode = QueryResultCode.Ok,
            Data = new LoginDetails()
            {
                UserID = user.UserID,
                Branches = user.Branches,
                Token = token
            }
        });
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public ActionResult<Response<LoginDetails>> RefreshToken([FromBody] Request<PayLoads.RefreshToken> request)
    {
        try
        {
            var newToken = _tokenAuthService.RefreshToken(request.Data.Token);
        
            return Ok(new Response<LoginDetails>
            {
                Success = true,
                Message = "Token refreshed successfully",
                StatusCode = QueryResultCode.Ok,
                Data = new LoginDetails
                {
                    Token = newToken
                }
            });
        }
        catch (SecurityTokenException)
        {
            return Unauthorized(new Response<LoginDetails>
            {
                Success = false,
                StatusCode = QueryResultCode.Unauthorized,
                Message = "Invalid token"
            });
        }
    }
    
    /// <summary>
    /// Verifies a Google reCAPTCHA v3 token with Google's verification API
    /// </summary>
    /// <param name="token">The reCAPTCHA token to verify</param>
    /// <param name="action">The action that was being performed</param>
    /// <returns>True if the token is valid, false otherwise</returns>
    private async Task<bool> VerifyReCaptchaTokenAsync(string token, string action)
    {
        try
        {
            // Get the reCAPTCHA secret key from configuration
            var secretKey = _config["ReCaptcha:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                // If no secret key is configured, allow the request in development
                // In production, you would reject the request
                #if DEBUG
                return true;
                #else
                return false;
                #endif
            }
            
            // Create the HTTP client
            var client = _httpClientFactory.CreateClient();
            
            // Prepare the verification request
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "secret", secretKey },
                { "response", token }
            });
            
            // Send the verification request to Google
            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            // Parse the response
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;
            
            // Check if the verification was successful
            if (root.TryGetProperty("success", out var success) && success.GetBoolean())
            {
                // Verify the action matches what we expect
                if (root.TryGetProperty("action", out var responseAction) && 
                    responseAction.GetString() == action)
                {
                    // Check the score (0.0 to 1.0)
                    if (root.TryGetProperty("score", out var score))
                    {
                        // Score threshold can be adjusted based on your security needs
                        return score.GetDouble() >= 0.5;
                    }
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"reCAPTCHA verification error: {ex.Message}");
            
            // In development, allow the request if verification fails
            // In production, you would reject the request
            #if DEBUG
            return true;
            #else
            return false;
            #endif
        }
    }
}