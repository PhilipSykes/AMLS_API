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
    
    /// <summary>
    /// Initializes a new instance of the AuthController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="authSearchRepo">Service for user search operations</param>
    /// <param name="tokenAuthService">Service for JWT token operations</param>
    /// <param name="otpService">Service for OTP code management</param>
    public AuthController(
        Exchange exchange, 
        ISearchRepository<Login> authSearchRepo, 
        TokenAuthService tokenAuthService,
        OtpService otpService)
    {
        _exchange = exchange;
        _authSearchRepo = authSearchRepo;
        _tokenAuthService = tokenAuthService;
        _otpService = otpService;
    }

    /// <summary>
    /// Authenticates a user and sends an OTP code via email
    /// </summary>
    /// <param name="request">Login credentials containing email and password</param>
    /// <returns>Response indicating successful authentication and OTP code sent</returns>
    [HttpPost("login")]
    public async Task<ActionResult<Response<object>>> Login([FromBody] Request<PayLoads.Login> request)
    {
        var emailFilter = new List<Filter>
        {
            new Filter(DbFieldNames.Login.Email, request.Data.Email, DbEnums.Equals)
        };
        List<Login> result = await _authSearchRepo.Search(DocumentTypes.Login, emailFilter);

        if (!result.Any())
        {
            return Unauthorized(new Response<object>
            {
                Success = false,
                StatusCode = QueryResultCode.BadRequest,
                Message = "No Results Found",
            });
        }
        
        if (!PasswordService.VerifyPassword(result[0].PasswordHash, request.Data.Password))
        {
            return Unauthorized(new Response<object>
            {
                Success = false,
                StatusCode = QueryResultCode.Unauthorized,
                Message = "Invalid Credentials",
            });
        }
        
        // Generate one-time code
        string otpCode = _otpService.GenerateOtp(result[0]);
        
        // Send OTP email instead of login confirmation
        var emailDetails = new EmailDetails
        {
            UserId = result[0].UserID,
            RecipientAddresses = new List<string> { result[0].Email },
            EmailBody = new Dictionary<string, string>
            {
                { "UserName", result[0].Email },
                { "Code", otpCode }
            }
        };
        
        // Send verification code email
        await _exchange.PublishNotification(
            MessageTypes.EmailNotifications.TwoFactorCode, 
            emailDetails);
            
        return Ok(new Response<object>
        {
            Success = true,
            Message = "Verification code sent to your email",
            StatusCode = QueryResultCode.Ok,
            Data = new 
            {
                Email = result[0].Email,
                RequiresTwoFactorVerification = true
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
}