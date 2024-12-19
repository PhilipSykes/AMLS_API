using Common.Constants;
using Common.Database;
using Common.MessageBroker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReservationService;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = Policies.CanBorrowMedia)]
public class LoansController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IReservationRepository _reservationRepository;
    
    /// <summary>
    /// Initializes a new instance of the LoansController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="loanManager">Loan manager service</param>
    public LoansController(Exchange exchange, IReservationRepository reservationRepository)
    {
        _exchange = exchange;
        _reservationRepository = reservationRepository;
    }
    
    [HttpPost("check-in")]
    public async Task<ActionResult> CheckIn(string physicalId)
    {
        
        var result = await _reservationRepository.CheckIn(physicalId);
    
        return Ok(new { message = result.StatusCode });
    }
    
    [HttpPost("check-out")]
    public async Task<ActionResult> CheckOut((string, string) request)
    {
        var physicalId = request.Item1;
        var memberId = request.Item2;
        var result = await _reservationRepository.CheckOut(physicalId, memberId);
    
        if (!result.Success) return Conflict(new { message = "Media is reserved by another" });
    
        return Created("",new {message = "Reservation created and media is borrowed" });
    }
}