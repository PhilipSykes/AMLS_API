using Common.Constants;
using Common.Database;
using Common.Database.Interfaces;
using Common.Models;
using MongoDB.Bson;

namespace Services.SearchService
{
    public interface IMediaSearchService
    {
        Task<SearchResponse> SearchMedia(List<Filter> filters);
        Task<SearchResponse> GetInitialMedia(); 
    }

    public class MediaSearchService : IMediaSearchService 
    {
        private readonly ISearchRepository _searchRepository;

        public MediaSearchService(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<SearchResponse> SearchMedia(List<Filter> filters)
        {
                Console.WriteLine($"Performing media search with {filters.Count} filters");
                var response = await _searchRepository.Search(filters,DocumentTypes.MediaInfo);
                Console.WriteLine($"Search completed. Found {response.TotalCount} results");
                return response; 
        }

        public async Task<SearchResponse> GetInitialMedia()
        {
                Console.WriteLine("Fetching initial media results");
                var response = await _searchRepository.Search(new List<Filter>(),DocumentTypes.MediaInfo);
                Console.WriteLine($"Initial fetch completed. Found {response.TotalCount} results");
                return response; 
        }
    }
}