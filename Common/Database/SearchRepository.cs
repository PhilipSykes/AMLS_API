using Common.Constants;
using static Common.Models.Shared;
using static Common.Models.Operations;
using Common.Exceptions;
using Common.Models;
using Common.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.Database
{
    
    public interface ISearchRepository<T> where T : class
    {
        Task<PaginatedResponse<List<T>>> PaginatedSearch(string documentType, (int, int)pagination, List<Filter> filters = null,AgreggateSearchConfig config = null);
        Task<List<T>> Search(string documentType, List<Filter> filters = null);
    }

    public class SearchRepository<T> : ISearchRepository<T> where T : class
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

        public async Task<PaginatedResponse<List<T>>> PaginatedSearch(string documentType, (int, int) pagination, List<Filter> filters,AgreggateSearchConfig config)
        {
            try
            {
                List<BsonDocument> result;
                var collection = _database.GetCollection<BsonDocument>(documentType);
                var filterDefinition = _filterBuilder.BuildFilter(filters);

                var matches = collection.CountDocumentsAsync(filterDefinition);
                if (config?.UseAggregation == true)
                {
                    result = await ApplyAggregate(collection, filterDefinition, pagination, config);
                }
                else
                {
                    result =  await collection
                        .Find(filterDefinition)
                        .Skip(pagination.Item1)
                        .Limit(pagination.Item2)
                        .ToListAsync();
                }
                
                List<T> convertedResult = BsonDTOMapper.ConvertBsonToEntity<T>(result);
                
                return new PaginatedResponse<List<T>>
                {
                    Data = convertedResult,
                    Success = true,
                    MatchCount = matches.Result,
                    StatusCode = QueryResultCode.Ok
                };
            }
            catch (MongoException ex)
            {
                return new PaginatedResponse<List<T>>
                {
                    Success = false,
                    Message = ex.Message,
                    StatusCode = QueryResultCode.InternalServerError
                };
            }
        }

        public async Task<List<T>> Search(string documentType, List<Filter> filters = null)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>(documentType);
                var documents = await collection.Find(_filterBuilder.BuildFilter(filters)).ToListAsync();
                return BsonDTOMapper.ConvertBsonToEntity<T>(documents);
            }
            catch (MongoException)
            {
                throw new SearchException(SearchException.SearchErrorType.Database);
            }
        }

        public async Task<List<BsonDocument>> ApplyAggregate(IMongoCollection<BsonDocument> collection,
            FilterDefinition<BsonDocument> filterDefinition,(int, int) pagination, AgreggateSearchConfig Config)
        {
            return await collection.Aggregate()
                .Match(filterDefinition)
                .Lookup(Config.LookupCollection, Config.LocalField, Config.ForeignField, Config.As)
                .Project(Config.ProjectionString)
                .Skip(pagination.Item1)
                .Limit(pagination.Item2)
                .ToListAsync();
        }
    }
}