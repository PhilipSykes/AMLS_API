using System.Text.Json;
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
        if (request.EmailDetails.RecipientAddresses.Count == 0)
        {
            return BadRequest("Email recipients required");
        }
        
        //TODO actual reservation operation

        await _exchange.PublishNotification(
            MessageTypes.EmailNotifications.ReserveMedia, 
            request.EmailDetails);
    
        return Ok(new { message = "Reservation made" });
    }
}