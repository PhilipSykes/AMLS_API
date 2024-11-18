using Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.Database.Interfaces;

public class BsonFilterBuilder : IFilterBuilder<BsonDocument>
{
    private enum Operations
    {
        Equals = '=',
        NotEquals = '!',
        GreaterThan = '>',
        LessThan = '<',
        Contains = '~'
    }

    public FilterDefinition<BsonDocument> BuildFilter(List<Filter> filterObjectsIn)
    {
        var builder = Builders<BsonDocument>.Filter;
        
        if (filterObjectsIn is null) // Allows filterless queries
        {
            return builder.Empty;
        }
        
        var mongoFilters = new List<FilterDefinition<BsonDocument>>();

        foreach (var filterObject in filterObjectsIn)
        {
            switch (filterObject.Operation)
            {
                case (char)Operations.Equals:
                    mongoFilters.Add(builder.Eq(filterObject.Key, filterObject.Value));
                    break;
                case (char)Operations.GreaterThan:
                    mongoFilters.Add(builder.Gt(filterObject.Key, filterObject.Value));
                    break;
                case (char)Operations.LessThan:
                    mongoFilters.Add(builder.Lt(filterObject.Key, filterObject.Value));
                    break;
                case (char)Operations.NotEquals:
                    mongoFilters.Add(builder.Not(builder.Eq(filterObject.Key, filterObject.Value)));
                    break;
                case (char)Operations.Contains:
                    if (filterObject.Value is string) // This is a temp hack, fix properly later
                    {
                        mongoFilters.Add(builder.Regex(filterObject.key, new BsonRegularExpression($".*{filterObject.value}.*", "i"))); 
                    }
                    else
                    {
                        mongoFilters.Add(builder.AnyEq(filterObject.key, (string)filterObject.value));
                    }
                    break;
            }
        }

        return mongoFilters.Count > 0 ? builder.And(mongoFilters) : builder.Empty;
    }
}
