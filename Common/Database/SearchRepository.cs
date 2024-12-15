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
        Task<PaginatedResponse<List<T>>> PaginatedSearch(string documentType, (int, int) pagination,
            List<Filter>? filters = null, AgreggateSearchConfig? config = null);

        Task<List<T>> Search(string documentType, List<Filter>? filters = null);
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

        public async Task<PaginatedResponse<List<T>>> PaginatedSearch(string documentType, (int, int) pagination,
            List<Filter>? filters, AgreggateSearchConfig? config)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>(documentType);

                if (config != null)
                {
                    return await HandleAggregationSearch(collection, pagination, filters, config);
                }

                return await HandleSimpleSearch(collection, pagination, filters);

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

        private async Task<PaginatedResponse<List<T>>> HandleSimpleSearch(IMongoCollection<BsonDocument> collection,
            (int, int) pagination, List<Filter>? filters)
        {
            var filterDefinition = _filterBuilder.BuildFilter(filters);
            var matches = await collection.CountDocumentsAsync(filterDefinition);

            var result = await collection
                .Find(filterDefinition)
                .Skip(pagination.Item1)
                .Limit(pagination.Item2)
                .ToListAsync();

            List<T> convertedResult = BsonDTOMapper.ConvertBsonToEntity<T>(result);

            return new PaginatedResponse<List<T>>
            {
                Data = convertedResult,
                Success = true,
                MatchCount = matches,
                StatusCode = QueryResultCode.Ok
            };
        }

        private async Task<PaginatedResponse<List<T>>> HandleAggregationSearch(
            IMongoCollection<BsonDocument> collection, (int, int) pagination,
            List<Filter>? filters, AgreggateSearchConfig config)
        {
            var (preFilters, postFilters) = _filterBuilder.SplitFilters(filters);
            
            var pipelineDefinition = CreateBasePipeline(collection, preFilters, postFilters, config);
            
            var matches = await pipelineDefinition.ToListAsync();
            
            if (!string.IsNullOrEmpty(config.ProjectionString))
            {
                pipelineDefinition = pipelineDefinition.Project(config.ProjectionString);
            }

            var results = await pipelineDefinition
                .Skip(pagination.Item1)
                .Limit(pagination.Item2)
                .ToListAsync();

            List<T> convertedResult = BsonDTOMapper.ConvertBsonToEntity<T>(results);

            return new PaginatedResponse<List<T>>
            {
                Data = convertedResult,
                Success = true,
                MatchCount = matches.Count,
                StatusCode = QueryResultCode.Ok
            };
        }

        private IAggregateFluent<BsonDocument> CreateBasePipeline(IMongoCollection<BsonDocument> collection,
            List<Filter>? preFilters, List<Filter>? postFilters, AgreggateSearchConfig config)
        {
            var pipeline = collection.Aggregate();
            
            if (preFilters != null && preFilters.Any())
            {
                pipeline = pipeline.Match(_filterBuilder.BuildFilter(preFilters));
            }
            
            for (int i = 0; i < config.LookupCollections.Count; i++)
            {
                pipeline = pipeline.Lookup(
                    config.LookupCollections[i],
                    config.LocalFields[i],
                    config.ForeignFields[i],
                    config.OutputFields[i]
                );
            }
            
            if (postFilters != null && postFilters.Any())
            {
                pipeline = pipeline.Match(_filterBuilder.BuildFilter(postFilters));
            }

            return pipeline;
        }

        public async Task<List<T>> Search(string documentType, List<Filter>? filters = null)
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


    }
}