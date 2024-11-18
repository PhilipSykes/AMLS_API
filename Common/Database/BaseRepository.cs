using Common.Models;
using Common.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.Database
{
    public abstract class BaseRepository
    {
        protected readonly IMongoDatabase Database;
        protected readonly IFilterBuilder<BsonDocument> FilterBuilder;

        protected BaseRepository(IOptions<MongoDBConfig> options)
        {
            var config = options.Value;
            var client = new MongoClient(config.ConnectionString);
            Database = client.GetDatabase(config.DatabaseName);
            FilterBuilder = new BsonFilterBuilder();
        }

        protected async Task<SearchResponse> Search(
            IMongoCollection<BsonDocument> collection, 
            List<Filter> filters)
        {
            try
            {
                var results = await collection.Find(FilterBuilder.BuildFilter(filters)).ToListAsync();
                var jsonStrings = results.Select(doc => doc.ToJson()).ToList();
                return new SearchResponse 
                { 
                    Results = jsonStrings,
                    TotalCount = results.Count
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search error: {ex.Message}");
                return new SearchResponse 
                { 
                    Results = new List<string>(),
                    Error = "Failed to perform search"
                };
            }
        }
    }
}