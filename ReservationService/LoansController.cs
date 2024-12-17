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
    public async Task<ActionResult> CheckOut(string physicalId, string memberId)
    {
        
        var result = await _reservationRepository.CheckOut(physicalId, memberId);
    
        return Ok(new { message = result.StatusCode });
    }
}