using Common.Constants;
using Common.Database.Interfaces;
using Common.Exceptions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using static Common.Models.Shared;

namespace Common.Database;

public interface ISearchRepository
{
    Task<List<BsonDocument>> Search(string documentType, (int, int) pagination, List<Filter> filters = null);
    Task<List<BsonDocument>> Search(string documentType, List<Filter> filters = null);
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

    public async Task<List<BsonDocument>> Search(string documentType, (int, int) pagination, List<Filter> filters)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>(documentType);

            return await collection.Aggregate()
                .Match(_filterBuilder.BuildFilter(filters))
                .Lookup(DocumentTypes.PhysicalMedia, "_id", "info", "physicalCopies")
                .Project(@"{  
                    'physicalCopies._id': 0, 
                    'physicalCopies.info': 0,
                    'physicalCopies.reservations': 0
                    }")
                .Skip(pagination.Item1)
                .Limit(pagination.Item2)
                .ToListAsync();
        }
        catch (MongoException)
        {
            throw new SearchException(SearchException.SearchErrorType.Database);
        }
    }

    public async Task<List<BsonDocument>> Search(string documentType, List<Filter> filters = null)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>(documentType);

            return await collection.Find(_filterBuilder.BuildFilter(filters)).ToListAsync();
        }
        catch (MongoException)
        {
            throw new SearchException(SearchException.SearchErrorType.Database);
        }
    }
}