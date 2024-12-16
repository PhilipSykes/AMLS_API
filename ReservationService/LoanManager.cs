using Common.Constants;
using Common.Database;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Common.Utils;
using MongoDB.Bson;

namespace ReservationService;


public interface ILoanManager
{
    public Task<Response<bool>> CheckOut(string physicalId, string memberId);
    public Task<Response<bool>> CheckIn(string physicalId); 

}
public class LoanManager : ILoanManager
{
    private readonly IReservationRepository _reservationRepository;

    public LoanManager(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Response<bool>> CheckOut(string physicalId, string memberId)
    {
        return await _reservationRepository.CheckOut(physicalId, memberId);
    }

    public async Task<Response<bool>> CheckIn(string physicalId)
    {
        return await _reservationRepository.CheckIn(physicalId);
        // Notify next user?
    }
}