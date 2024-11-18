using Common.Database;
using Common.Database.Interfaces;
using Common.Models;
using MongoDB.Bson;

namespace Services.SearchService
{
    public interface IMediaSearchService
    {
        Task<SearchResponse> SearchMediaAsync(List<Filter> filters);
        Task<SearchResponse> GetInitialMediaAsync(); 
    }

    public class MediaSearchService : IMediaSearchService 
    {
        private readonly ISearchRepository _searchRepository;

        public MediaSearchService(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<SearchResponse> SearchMediaAsync(List<Filter> filters)
        {
            try
            {
                Console.WriteLine($"Performing media search with {filters.Count} filters");
                var response = await _searchRepository.SearchMediaInfo(filters);
                Console.WriteLine($"Search completed. Found {response.TotalCount} results");
        
                return response; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error performing media search: {ex.Message}");
                return new SearchResponse 
                { 
                    Error = $"Failed to perform search: {ex.Message}",
                    Results = new List<BsonDocument>(),
                    TotalCount = 0
                };
            }
        }

        public async Task<SearchResponse> GetInitialMediaAsync()
        {
            try
            {
                Console.WriteLine("Fetching initial media results");
                var response = await _searchRepository.SearchMediaInfo(new List<Filter>());
                Console.WriteLine($"Initial fetch completed. Found {response.TotalCount} results");
        
                return response; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching initial results: {ex.Message}");
                return new SearchResponse
                {
                    Error = $"Failed to fetch initial results: {ex.Message}",
                    Results = new List<BsonDocument>(),
                    TotalCount = 0
                };
            }
        }
    }
}