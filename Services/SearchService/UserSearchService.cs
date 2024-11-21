using Common.Constants;
using Common.Database;
using Common.Models;

namespace Services.SearchService;

public interface IUserSearchService
{
    Task<SearchResponse> GetInitialUsers((int, int) pagination, List<Filter> filters);
    Task<SearchResponse> SearchUsers((int, int) pagination, List<Filter> filters);
    Task<SearchResponse> GetUserCredentials(List<Filter> filters); 
}

public class UserSearchService : IUserSearchService
{
    private readonly ISearchRepository _searchRepository;

    public UserSearchService(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<SearchResponse> GetUserCredentials(List<Filter> filters)
    {
        var response = await _searchRepository.Search(DocumentTypes.Login, filters);
        return response;
    }

    public async Task<SearchResponse> SearchUsers((int, int) pagination, List<Filter> filters)
    {
        Console.WriteLine($"Performing user search with {filters.Count} filters");
        var response = await _searchRepository.Search(DocumentTypes.Login, pagination, filters);
        Console.WriteLine($"Search completed. Found {response.TotalCount} results");
        return response;
    }

    public async Task<SearchResponse> GetInitialUsers((int, int) pagination, List<Filter> filters)
    {
        Console.WriteLine("Fetching initial user results");
        var response = await _searchRepository.Search(DocumentTypes.Login, pagination);
        Console.WriteLine($"Initial fetch completed. Found {response.TotalCount} results");
        return response; 
    }
}