using Api.MessageBroker;
using Common.Constants;
using Common.Models;
using static Common.Models.Operations;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Services.UserService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]  
public class UserController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IUserSearch _userSearch;
    
    public UserController(Exchange exchange, IUserSearch userSearch)
    {
        _exchange = exchange;
        _userSearch = userSearch;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] Request<PayLoads.Login> request)
    {
        Console.WriteLine($"Login request for user: {request.Data.Email}");
        
        var emailFilter = new List<Filter> 
        {
            new Filter(DbFieldNames.Login.Email, request.Data.Email, DbOperations.Equals)
        };
        Response<List<Entities.Login>> response = await _userSearch.GetLoginCredentials(emailFilter);
    
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

    public async Task HashAllPasswords()
    {
        var response = await _userSearch.GetLoginCredentials(null);
 
        var passwordService = new PasswordService();
        var updatedLogins = response.Data.Select(login => new Entities.Login
        {
            ObjectID = login.ObjectID,
            User = login.User,
            Email = login.Email,
            PasswordHash = passwordService.HashPassword(login.PasswordHash),
            Role = login.Role
        }).ToList();

        //TODO write method needed UpdateLoginCredentials(updatedLogins);
    }
}