using Common.Constants;
using Common.Database;
using Common.Database.Interfaces;
using Common.Models;
using MongoDB.Bson;

namespace Services.SearchService
{
    public interface IMediaSearchService
    {
        Task<SearchResponse> SearchMedia((int, int) pagination, List<Filter> filters);
        Task<SearchResponse> GetInitialMedia((int, int) pagination); 
    }

    public class MediaSearchService : IMediaSearchService 
    {
        private readonly ISearchRepository _searchRepository;

        public MediaSearchService(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<SearchResponse> SearchMedia((int, int) pagination, List<Filter> filters)
        {
                Console.WriteLine($"Performing media search with {filters.Count} filters");
                var response = await _searchRepository.Search(DocumentTypes.MediaInfo, pagination, filters);
                Console.WriteLine($"Search completed. Found {response.TotalCount} results");
                return response; 
        }

        public async Task<SearchResponse> GetInitialMedia((int, int) pagination)
        {
                Console.WriteLine("Fetching initial media results");
                var response = await _searchRepository.Search(DocumentTypes.MediaInfo, pagination);
                Console.WriteLine($"Initial fetch completed. Found {response.TotalCount} results");
                return response; 
        }
    }
}