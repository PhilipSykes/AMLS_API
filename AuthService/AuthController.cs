using Common.MessageBroker;
using Common.Constants;
using Common.Models;
using static Common.Models.Shared;
using static Common.Models.Operations;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Common.Database;
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
    
    /// <summary>
    /// Initializes a new instance of the AuthController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="authSearchRepo">Service for user search operations</param>
    /// <param name="tokenAuthService">Service for JWT token operations</param>
    public AuthController(Exchange exchange, ISearchRepository<Login> authSearchRepo,TokenAuthService tokenAuthService)
    {
        _exchange = exchange;
        _authSearchRepo = authSearchRepo;
        _tokenAuthService = tokenAuthService;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials containing email and password</param>
    /// <returns>Response containing login details and JWT token if successful</returns>
    [HttpPost("login")]
    public async Task<ActionResult<Response<LoginDetails>>> Login([FromBody] Request<PayLoads.Login> request)
    {
        Console.WriteLine($"Login request for user: {request.Data.Email}");

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
                Message = "No Results Found",
            });
        }

        var passwordService = new PasswordService();
            if (!passwordService.VerifyPassword(result[0].PasswordHash, request.Data.Password))
            {
                return Unauthorized(new Response<LoginDetails>
                {
                    Success = false,
                    Message = "Invalid Credentials",
                });
            }
            // await _exchange.PublishNotification(
            //     MessageTypes.EmailNotifications.Login, 
            //     request.EmailDetails);
            string token = _tokenAuthService.GenerateJwtToken(result[0]);
            Console.WriteLine($"token: {token}");
            foreach (string branch in result[0].Branches)
            {
                Console.WriteLine($"branch access: {branch}");
            }
            return Ok(new Response<LoginDetails>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginDetails()
                {
                    UserID = result[0].UserID,
                    Branches = result[0].Branches,
                    Token = token
                }
            });
    }
    
    
}