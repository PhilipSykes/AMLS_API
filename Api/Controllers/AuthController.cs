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
    
    public AuthController(Exchange exchange, IUserSearch userSearch)
    {
        _exchange = exchange;
        _userSearch = userSearch;
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
            //if (!passwordService.VerifyPassword(result[0].PasswordHash, request.Data.Password))
            if (result[0].PasswordHash != request.Data.Password)
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
            TokenAuthService auth = new TokenAuthService();
            string token = auth.GenerateJwtToken(result[0]);
            Console.WriteLine($"token: {token}");
            return Ok(new Response<LoginDetails>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginDetails()
                {
                    Username = result[0].UserId,
                    Token = token
                }
            });
    }
    
    public async Task HashAllPasswords() //TEMP 
    {
        var response = await _userSearch.GetLoginCredentials(null);
 
        var passwordService = new PasswordService();
        var updatedLogins = response.Select(login => new Entities.Login
        {
            ObjectID = login.ObjectID,
            UserId = login.UserId,
            Email = login.Email,
            PasswordHash = passwordService.HashPassword(login.PasswordHash),
            Role = login.Role
        }).ToList();

        //TODO write method needed UpdateLoginCredentials(updatedLogins);
    }
    
}