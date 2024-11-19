using Api.MessageBroker;
using Common.Constants;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]  
public class MediaController : ControllerBase
{
    private readonly Exchange _exchange;
    
    public MediaController(Exchange exchange)
    {
        _exchange = exchange;
    }

    [HttpPost("reserve")]
    public async Task<ActionResult> Reserve([FromBody] ReserveRequest request)
    {
        Console.WriteLine($"Reserve media request for user: {request.UserId}");
        
        // TODO: create reserve media operations & implement logic 
        
        
        await _exchange.PublishNotification(
            MessageTypes.EmailNotifications.ReserveMedia, 
            request.EmailDetails); 
        
        return Ok(new { message = "Reservation made" });
    }
}