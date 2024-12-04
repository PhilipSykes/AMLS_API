using Api.MessageBroker;
using Common.Constants;
using Microsoft.AspNetCore.Authorization;
using static Common.Models.Operations;
using static Common.Models.PayLoads;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/reservations")]
[Authorize(Policy = Policies.RequireMember)]
public class ReservationsController : ControllerBase
{
    private readonly Exchange _exchange;
    
    public ReservationsController(Exchange exchange)
    {
        _exchange = exchange;
    }
    
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Request<Reserve> request)
    {
        if (request.EmailDetails.RecipientAddresses.Count == 0)
        {
            return BadRequest("Email recipients required");
        }
        //TODO actual reservation operation: create reservation in reservations table, flag physicalmedia as reserved
        
        await _exchange.PublishNotification(
            MessageTypes.EmailNotifications.ReserveMedia, 
            request.EmailDetails);
    
        return Ok(new { message = "Reservation made" });
    }
}