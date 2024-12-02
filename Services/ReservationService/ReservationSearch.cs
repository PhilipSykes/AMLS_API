using Common.Constants;
using Common.Database;
using Common.Utils;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;

namespace Services.ReservationService;

public interface IReservationSearch
{
    Task<Response<List<Reservations>>> SearchReservations((int, int) pagination, List<Filter> filters);
}

public class ReservationSearch : IReservationSearch
{
    private readonly ISearchRepository _searchRepository;

    public ReservationSearch(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<Response<List<Reservations>>> SearchReservations((int, int) pagination,
        List<Filter> filters)
    {
        //Console.WriteLine($"Performing media search with {filters.Count} filters");
        var bsonDocuments = await _searchRepository.Search(DocumentTypes.Reservations, pagination, filters);

        List<Reservations> reservationsList = Utils.ConvertBsonToEntity<Reservations>(bsonDocuments);

        Console.WriteLine($"Search completed. Found {reservationsList.Count} results");
        return new Response<List<Reservations>>
        {
            Success = true,
            Data = reservationsList
        };
    }
}