using Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Common.Constants;

namespace Common.Database.Interfaces;

public class BsonFilterBuilder : IFilterBuilder<BsonDocument>
{
  
    public FilterDefinition<BsonDocument> BuildFilter(List<Filter> filterObjectsIn)
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
                        mongoFilters &= builder.Regex(filterObject.Key, new BsonRegularExpression($".*{filterObject.Value}.*", "i")); 
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
}
