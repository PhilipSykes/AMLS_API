using Common.Constants;
using Common.Database;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Common.Utils;
using MongoDB.Bson;

namespace ReservationService;


public interface IReservationCreator
{
    Task<Response<bool>> CreateReservation(Reservation reservation);
    Task<Response<bool>> ExtendReservation(string reservationId, DateTime newEndDate);
    Task<Response<bool>> CancelReservation(string reservationId);
    
}
public class ReservationCreator : IReservationCreator
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationCreator(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Response<bool>> CreateReservation(Reservation reservation)
    {
        return await _reservationRepository.CreateReservation(reservation);
    }

    public async Task<Response<bool>> ExtendReservation(string reservationId, DateTime endDate)
    {
        return await _reservationRepository.ExtendReservation(reservationId, endDate);
    }

    public async Task<Response<bool>> CancelReservation(string reservationId)
    {
        return await _reservationRepository.CancelReservation(reservationId);
    }
    
}