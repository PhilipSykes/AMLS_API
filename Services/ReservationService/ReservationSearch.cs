using Common.Constants;
using Common.Database;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Common.Utils;
using MongoDB.Bson;

namespace Services.ReservationService
{
    public interface IReservationSearch
    {
        Task<Response<List<Reservation>>> SearchReservations((int, int) pagination, List<Filter> filters);
    }

    public class ReservationSearch : IReservationSearch
    {
        private readonly ISearchRepository _searchRepository;
        private readonly IReservationRepository _reservationRepository;

        public ReservationSearch(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<Response<List<Reservation>>> SearchReservations((int, int) pagination,
            List<Filter> filters)
        {
            //Console.WriteLine($"Performing media search with {filters.Count} filters");
            var result = await _searchRepository.PaginatedSearch(DocumentTypes.Reservations, pagination, filters);
            
            List<Reservation> reservationsList = Utils.ConvertBsonToEntity<Reservation>(result.Data);

            Console.WriteLine($"Search completed. Found {reservationsList.Count} results");
            return new PaginatedResponse<List<Reservation>>
            {
                Success = true,
                Data = reservationsList
            };
        }
        
    }
}
