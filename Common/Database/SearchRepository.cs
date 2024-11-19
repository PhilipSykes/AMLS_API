using Common.Models;
using Common.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.Database
{
    public interface ISearchRepository
    {
        Task<SearchResponse> Search(List<Filter> filters, string documentType);
    }

    public class SearchRepository : ISearchRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IFilterBuilder<BsonDocument> _filterBuilder;

        public SearchRepository(IOptions<MongoDBConfig> options)
        {
            var config = options.Value;
            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
            _filterBuilder = new BsonFilterBuilder();
        }

        public async Task<SearchResponse> Search(List<Filter> filters, string documentType)
        {
            var collection = _database.GetCollection<BsonDocument>(documentType);
            try
            {
                List<BsonDocument> results = await collection.Find(_filterBuilder.BuildFilter(filters)).ToListAsync();
                List<string> jsonStrings = results.Select(doc => doc.ToJson()).ToList();
                return new SearchResponse
                {
                    Results = jsonStrings,
                    TotalCount = results.Count
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{documentType} Search error: {ex.Message}");
                return new SearchResponse
                {
                    Results = new List<string>(),
                    Error = "Failed to perform search"
                };
            }

        }
    }
}
