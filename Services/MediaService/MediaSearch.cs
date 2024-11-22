using Common.Constants;
using Common.Database;
using Common.Models;
using Common.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Services.MediaService
{
    public interface IMediaSearch
    {
        Task<Operations.Response<List<Entities.MediaInfo>>> SearchMedia((int, int) pagination, List<Filter> filters);
    }

    public class MediaSearch : IMediaSearch
    {
        private readonly ISearchRepository _searchRepository;

        public MediaSearch(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<Operations.Response<List<Entities.MediaInfo>>> SearchMedia((int, int) pagination,
            List<Filter> filters)
        {
            //Console.WriteLine($"Performing media search with {filters.Count} filters");
            List<BsonDocument> bsonDocuments = await _searchRepository.Search(DocumentTypes.MediaInfo, pagination, filters);
            
            List<Entities.MediaInfo> mediaInfoList = Utils.ConvertBsonToEntity<Entities.MediaInfo>(bsonDocuments);

            Console.WriteLine($"Search completed. Found {mediaInfoList.Count} results");
            return new Operations.Response<List<Entities.MediaInfo>>
            {
                Success = true,
                Data = mediaInfoList
            };
        }
    }
}
