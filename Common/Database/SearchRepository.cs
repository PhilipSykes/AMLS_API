using Common.Constants;
using Common.Models;
using Common.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Common.Database
{
    public interface ISearchRepository
    {
        Task<List<BsonDocument>> Search(string documentType, (int, int)pagination, List<Filter> filters = null);
        Task<List<BsonDocument>> Search(string documentType, List<Filter> filters = null);
        Task<List<T>> ConvertBsonToEntity<T>(List<BsonDocument> bsonDocuments);
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

        public async Task<List<BsonDocument>> Search(string documentType, (int, int) pagination, List<Filter> filters = null)
        {
            var collection = _database.GetCollection<BsonDocument>(documentType);
            try
            {
                return await collection.Find(_filterBuilder.BuildFilter(filters)).Skip(pagination.Item1).Limit(pagination.Item2).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{documentType} Search error: {ex.Message}");
                throw;
            }

        }
        
        public async Task<List<BsonDocument>> Search(string documentType, List<Filter> filters = null)
        {
            var collection = _database.GetCollection<BsonDocument>(documentType);
            try
            {
                return await collection.Find(_filterBuilder.BuildFilter(filters)).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{documentType} Search error: {ex.Message}");
                throw;
            }

        }

        public async Task<List<T>> ConvertBsonToEntity<T>(List<BsonDocument> bsonDocuments)
        {
           List<T>entities  = bsonDocuments
                .Select(doc => BsonSerializer.Deserialize<T>(doc))
                .ToList();
           return entities;
        }
    }
}
