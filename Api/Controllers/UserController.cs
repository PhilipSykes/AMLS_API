using Api.MessageBroker;
using Common.Constants;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]  
public class UserController : ControllerBase
{
    private readonly Exchange _exchange;
    
    public UserController(Exchange exchange)
    {
        _exchange = exchange;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        Console.WriteLine($"Login request for user: {request.UserId}");
        
        // TODO: Actual login logic here
        
        await _exchange.PublishNotification(
            MessageTypes.EmailNotifications.Login, 
            request.EmailDetails);
        
        return Ok(new { message = "Login successful" });
    }
}