using Api.MessageBroker;
using Common.Constants;
using Common.Models;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Services.SearchService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]  
public class UserController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IUserSearchService _userSearchService;
    
    public UserController(Exchange exchange, IUserSearchService userSearchService)
    {
        _exchange = exchange;
        _userSearchService = userSearchService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] Operations.Request<PayLoads.Login> request)
    {
        Console.WriteLine($"Login request for user: {request.Data.Email}");
        
        var emailFilter = new List<Filter> 
        {
            new Filter(DBFieldNames.Login.Email, request.Data.Email, DbOperations.Equals)
        };
        var response = await _userSearchService.GetLoginCredentials(emailFilter);
    
        if (!response.Success || !response.Data.Any())
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var passwordService = new PasswordService();
        if (!passwordService.VerifyPassword(response.Data[0].PasswordHash, request.Data.Password))
        {
            return Unauthorized(new { message = response.Error });
        }
        
        // await _exchange.PublishNotification(
        //     MessageTypes.EmailNotifications.Login, 
        //     request.EmailDetails);
        // var cookieService = new CookieService();
        // cookieService.SetUserCookie(HttpContext, response.Data[0].User.ToString());
        return Ok(new { message = "Login successful",response.Data[0].User});
    }
}