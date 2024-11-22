using Common.Constants;
using Common.Database;
using Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Services.SearchService
{
    public interface IMediaSearchService
    {
        Task<Operations.Response<List<Entities.MediaInfo>>> SearchMedia((int, int) pagination, List<Filter> filters);
    }

    public class MediaSearchService : IMediaSearchService
    {
        private readonly ISearchRepository _searchRepository;

        public MediaSearchService(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<Operations.Response<List<Entities.MediaInfo>>> SearchMedia((int, int) pagination,
            List<Filter> filters)
        {
            //Console.WriteLine($"Performing media search with {filters.Count} filters");
            List<BsonDocument> bsonDocuments = await _searchRepository.Search(DocumentTypes.MediaInfo, pagination, filters);
            
            List<Entities.MediaInfo> mediaInfoList = await _searchRepository.ConvertBsonToEntity<Entities.MediaInfo>(bsonDocuments);

            Console.WriteLine($"Search completed. Found {mediaInfoList.Count} results");
            return new Operations.Response<List<Entities.MediaInfo>>
            {
                Success = true,
                Data = mediaInfoList
            };
        }
    }
}
