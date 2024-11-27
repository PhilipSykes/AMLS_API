using Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Common.Constants;
using Common.Exceptions;

namespace Common.Database.Interfaces;

public class BsonFilterBuilder : IFilterBuilder<BsonDocument>
{
  
    public FilterDefinition<BsonDocument> BuildFilter(List<Filter> filterObjectsIn)
    {
        try // Note from Will: This try catch is redundant, as operators are enforced by the enum. Unless someone were to remove operations from that enum for some reason anyway.
        {
            var builder = Builders<BsonDocument>.Filter;

            if (filterObjectsIn is null) // Allows filterless queries
            {
                return builder.Empty;
            }


            var mongoFilters = Builders<BsonDocument>.Filter.Empty;

            foreach (var filterObject in filterObjectsIn)
            {
                switch (filterObject.Operation)
                {
                    case DbOperations.Equals:
                        mongoFilters &= builder.Eq(filterObject.Key, filterObject.Value);
                        break;
                    case DbOperations.GreaterThan:
                        mongoFilters &= builder.Gt(filterObject.Key, filterObject.Value);
                        break;
                    case DbOperations.LessThan:
                        mongoFilters &= builder.Lt(filterObject.Key, filterObject.Value);
                        break;
                    case DbOperations.NotEquals:
                        mongoFilters &= builder.Not(builder.Eq(filterObject.Key, filterObject.Value));
                        break;
                    case DbOperations.Contains:
                        if (filterObject.Value is string) // This is a temp hack, fix properly later
                        {
                            mongoFilters &= builder.Regex(filterObject.Key,
                                new BsonRegularExpression($".*{filterObject.Value}.*", "i"));
                        }
                        else
                        {
                            mongoFilters &= builder.AnyEq(filterObject.Key, (string)filterObject.Value);
                        }

                        break;
                }
            }

            return mongoFilters;
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
        {
            throw new SearchException(SearchException.SearchErrorType.Validation);
        }
    }
}
