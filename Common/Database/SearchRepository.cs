using Common.Constants;
using static Common.Models.Shared;
using Common.Exceptions;
using Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.Database
{
    public interface ISearchRepository
    {
        Task<Operations.PaginatedResponse<List<BsonDocument>>> PaginatedSearch(string documentType, (int, int)pagination, List<Filter> filters = null);
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

        public async Task<Operations.PaginatedResponse<List<BsonDocument>>> PaginatedSearch(string documentType, (int, int) pagination, List<Filter> filters)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>(documentType);
                var filterDefinition = _filterBuilder.BuildFilter(filters);

                var matches = collection.CountDocumentsAsync(filterDefinition);
                var result = collection.Aggregate()
                    .Match(filterDefinition)
                    .Lookup(DocumentTypes.PhysicalMedia, "_id", "info", "physicalCopies")
                    .Project(@"{  
                    'physicalCopies._id': 0, 
                    'physicalCopies.info': 0 
                    }")
                    .Skip(pagination.Item1)
                    .Limit(pagination.Item2)
                    .ToListAsync();

                await Task.WhenAll(result, matches);
                return new Operations.PaginatedResponse<List<BsonDocument>>
                {
                    Data = result.Result,
                    Success = true,
                    MatchCount = matches.Result,
                    StatusCode = QueryResultCode.Ok
                };
            }
            catch (MongoException ex)
            {
                //throw new SearchException(SearchException.SearchErrorType.Database);

                return new Operations.PaginatedResponse<List<BsonDocument>>
                {
                    Success = false,
                    Message = ex.Message,//TODO - Write a function that maps errors to response codes
                    StatusCode = QueryResultCode.InternalServerError
                };
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
}
