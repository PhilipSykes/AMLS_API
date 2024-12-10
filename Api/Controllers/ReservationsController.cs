using Api.MessageBroker;
using Common.Constants;
using Microsoft.AspNetCore.Authorization;
using static Common.Models.Operations;
using static Common.Models.PayLoads;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for managing media reservations
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize(Policy = Policies.CanReserveMedia)]
public class ReservationsController : ControllerBase
{
    private readonly Exchange _exchange;
    
    /// <summary>
    /// Initializes a new instance of the ReservationsController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    public ReservationsController(Exchange exchange)
    {
        _exchange = exchange;
    }
    
    /// <summary>
    /// Creates a new media reservation and sends notification email
    /// </summary>
    /// <param name="request">Reservation details including email recipients</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Request<Reserve> request)
    {
        if (request.EmailDetails.RecipientAddresses.Count == 0)
        {
            return BadRequest("Email recipients required");
        }
        //TODO: actual reservation operation: create reservation in reservations table, flag physicalmedia as reserved
        
        await _exchange.PublishNotification(
            MessageTypes.EmailNotifications.ReserveMedia, 
            request.EmailDetails);
    
        return Ok(new { message = "Reservation made" });
    }
}