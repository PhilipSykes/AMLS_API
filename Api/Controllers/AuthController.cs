using Api.MessageBroker;
using Common.Constants;
using Common.Models;
using static Common.Models.Shared;
using static Common.Models.Operations;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Services.UserService;
using Services.TokenAuthService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")] 
public class AuthController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IUserSearch _userSearch;
    private readonly TokenAuthService _tokenAuthService;
    public AuthController(Exchange exchange, IUserSearch userSearch,TokenAuthService tokenAuthService)
    {
        _exchange = exchange;
        _userSearch = userSearch;
        _tokenAuthService = tokenAuthService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<Response<LoginDetails>>> Login([FromBody] Request<PayLoads.Login> request)
    {
        Console.WriteLine($"Login request for user: {request.Data.Email}");

        var emailFilter = new List<Filter>
        {
            new Filter(DbFieldNames.Login.Email, request.Data.Email, DbOperations.Equals)
        };
        List<Entities.Login> result = await _userSearch.GetLoginCredentials(emailFilter);

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
            return Ok(new Response<LoginDetails>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginDetails()
                {
                    Username = result[0].Username,
                    Token = token
                }
            });
    }
    
    
}