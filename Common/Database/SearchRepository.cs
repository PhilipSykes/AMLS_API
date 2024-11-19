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

        //Todo - Evaluate need for base repo search function
        public async Task<SearchResponse> Search(string documentType,(int, int) pagination ,List<Filter> filters = null)
        {
            var collection = Database.GetCollection<BsonDocument>(documentType);
            return await Search(collection,pagination, filters);
        }

    }
}