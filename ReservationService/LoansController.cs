using Common.Constants;
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
    private readonly ILoanManager _loanManager;
    
    /// <summary>
    /// Initializes a new instance of the LoansController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="loanManager">Loan manager service</param>
    public LoansController(Exchange exchange, ILoanManager loanManager)
    {
        _exchange = exchange;
        _loanManager = loanManager;
    }
    
    [HttpPost("check-in")]
    public async Task<ActionResult> CheckIn(string physicalId)
    {
        
        var result = await _loanManager.CheckIn(physicalId);
    
        return Ok(new { message = result.StatusCode });
    }
    
    [HttpPost("check-out")]
    public async Task<ActionResult> CheckOut(string physicalId, string memberId)
    {
        
        var result = await _loanManager.CheckOut(physicalId, memberId);
    
        return Ok(new { message = result.StatusCode });
    }
}