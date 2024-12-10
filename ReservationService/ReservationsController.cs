using MessageBroker;
using Common;
using Common.Constants;
using Common.Database;
using Common.Models;
using static Common.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using static Common.Models.PayLoads;
using Microsoft.AspNetCore.Mvc;


namespace ReservationService;
/// <summary>
/// Controller for managing media reservations
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize(Policy = Policies.CanReserveMedia)]

public class ReservationsController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IReservationCreator _reservationCreator;
    private readonly ISearchRepository<Reservation> _reservationSearchRepo;
    
    /// <summary>
    /// Initializes a new instance of the ReservationsController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="reservationCreator">Reservation creator service</param>
    /// <param name="reservationSearchRepo">Service for searching reservation items</param>
    public ReservationsController(Exchange exchange, IReservationCreator reservationCreator,ISearchRepository<Reservation> reservationSearchRepo)
    {
        _exchange = exchange;
        _reservationCreator = reservationCreator;
        _reservationSearchRepo = reservationSearchRepo;
    }
    
    /// <summary>
    /// Creates a new media reservation and sends notification email
    /// </summary>
    /// <param name="request">Reservation details including email recipients</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [HttpPost("create")] // Change to POST after tests
    public async Task<ActionResult> Create(Reservation reservation)
    {
        //if (request.EmailDetails.RecipientAddresses.Count == 0)
        //{
        //    return BadRequest("Email recipients required");
        //}
        
        
        var result = await _reservationCreator.CreateReservation(reservation);
        
        //await _exchange.PublishNotification(
        //    MessageTypes.EmailNotifications.ReserveMedia, 
        //    request.EmailDetails);
    
        return Ok(new { message = result.StatusCode });
    }

    [HttpPost("cancel")]
    public async Task<ActionResult> Cancel(string id)
    {
        var result = await _reservationCreator.CancelReservation(id);
        
        return Ok(new { message = result.StatusCode });
    }

    [HttpPost("extend")]
    public async Task<ActionResult> Extend(ReservationExtension request)
    {
        string id = request.ReservationId;
        DateTime newEndDate = request.NewEndDate;
        var result = await _reservationCreator.ExtendReservation(id, newEndDate);
        
        return Ok(new { message = result.StatusCode });
    }
}