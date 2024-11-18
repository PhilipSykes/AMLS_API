using Common.Constants;
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

        public async Task<SearchResponse> Search(List<Filter> filters,string documentType)
        {
            var collection = Database.GetCollection<BsonDocument>(documentType);
            return await Search(collection, filters);
        }

    }
}