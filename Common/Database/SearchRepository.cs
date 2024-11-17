using Common.Models;
using Common.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Common.Database
{
    public class SearchRepository : BaseRepository, ISearchRepository
    {
        public SearchRepository(IOptions<MongoDBConfig> options) 
            : base(options)
        {
        }

        public async Task<SearchResponse> SearchMediaInfo(List<Filter> filters)
        {
            var collection = Database.GetCollection<BsonDocument>("MediaInfo");
            return await Search(collection, filters);
        }

        public async Task<SearchResponse> SearchPhysicalMedia(List<Filter> filters = null)
        {
            var collection = Database.GetCollection<BsonDocument>("PhysicalMedia");
            return await Search(collection, filters ?? new List<Filter>());
        }
    }
}