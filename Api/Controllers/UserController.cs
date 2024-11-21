using Api.MessageBroker;
using Common.Constants;
using Common.Models;
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
        //Console.WriteLine($"Login request for user: {request.Data.Username}");
        
        
        var response = await _userSearchService.GetLoginCredentials(request.SearchFilters);
            
        //Console.WriteLine(response.Results);
        if (!string.IsNullOrEmpty(response.Error))
        {
            Console.WriteLine($"User credentials not found: {response.Error}");
            return StatusCode(500, response);
        }
        
        // await _exchange.PublishNotification(
        //     MessageTypes.EmailNotifications.Login, 
        //     request.EmailDetails);
        
        return Ok(new { message = "Login successful" });
    }
}