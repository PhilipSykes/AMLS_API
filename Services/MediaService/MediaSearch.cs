using Common.Constants;
using Common.Database;
using Common.Models;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Common.Utils;
using MongoDB.Bson;

namespace Services.MediaService
{
    public interface IMediaSearch
    {
        Task<Response<List<MediaInfo>>> SearchMedia((int, int) pagination, List<Filter> filters);
    }

    public class MediaSearch : IMediaSearch
    {
        private readonly ISearchRepository _searchRepository;

        public MediaSearch(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<Response<List<MediaInfo>>> SearchMedia((int, int) pagination,
            List<Filter> filters)
        {
            //Console.WriteLine($"Performing media search with {filters.Count} filters");
            List<BsonDocument> bsonDocuments = await _searchRepository.Search(DocumentTypes.MediaInfo, pagination, filters);
            
            List<MediaInfo> mediaInfoList = Utils.ConvertBsonToEntity<MediaInfo>(bsonDocuments);

            Console.WriteLine($"Search completed. Found {mediaInfoList.Count} results");
            Console.WriteLine($"{mediaInfoList.First().PhysicalCopies.First().Branch}");
            return new Response<List<MediaInfo>>
            {
                Success = true,
                Data = mediaInfoList
            };
        }
    }
}
