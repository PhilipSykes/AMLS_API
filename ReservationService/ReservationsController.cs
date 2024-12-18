using Common.MessageBroker;
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
    private readonly IReservationRepository _reservationRepository;
    private readonly ISearchRepository<Reservation> _reservationSearchRepo;
    
    /// <summary>
    /// Initializes a new instance of the ReservationsController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="reservationCreator">Reservation creator service</param>
    /// <param name="reservationSearchRepo">Service for searching reservation items</param>
    public ReservationsController(Exchange exchange, IReservationRepository reservationRepository,ISearchRepository<Reservation> reservationSearchRepo)
    {
        _exchange = exchange;
        _reservationRepository = reservationRepository;
        _reservationSearchRepo = reservationSearchRepo;
    }
    
    /// <summary>
    /// Creates a new media reservation and sends notification email
    /// </summary>
    /// <param name="request">Reservation details including email recipients</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [Authorize(Policy = Policies.CanReserveMedia)]
    [HttpPost("create")]
    public async Task<ActionResult> Create(Reservation reservation)
    {
        //if (request.EmailDetails.RecipientAddresses.Count == 0)
        //{
        //    return BadRequest("Email recipients required");
        //}
        
        
        var result = await _reservationRepository.CreateReservation(reservation);
        
        //await _exchange.PublishNotification(
        //    MessageTypes.EmailNotifications.ReserveMedia, 
        //    request.EmailDetails);
    
        return Ok(new { message = result.StatusCode });
    }

    [Authorize(Policy = Policies.CanCancelMedia)]
    [HttpPost("cancel")]
    public async Task<ActionResult> Cancel(string id)
    {
        var result = await _reservationRepository.CancelReservation(id);
        
        return Ok(new { message = result.StatusCode });
    }

    [Authorize(Policy = Policies.CanExtendReservation)]
    [HttpPost("extend")]
    public async Task<ActionResult> Extend(ReservationExtension request)
    {
        string id = request.ReservationId;
        DateTime newEndDate = request.NewEndDate;
        var result = await _reservationRepository.ExtendReservation(id, newEndDate);
        
        return Ok(new { message = result.StatusCode });
    }

    [Authorize(Policy = Policies.CanReserveMedia)]
    [HttpPost("getReservable")]
    public async Task<ActionResult<Operations.Response<List<ReservableItem>>>> GetReservableItems(Shared.GetReservablesRequest request)
    {
        var result = await _reservationRepository.GetReservableItems(request.Media, request.Branches, request.MinimumDays);

        return result;
    }
}