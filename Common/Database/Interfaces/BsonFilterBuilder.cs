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

    public FilterDefinition<BsonDocument> BuildFilter(List<Filter> filtersIn)
    {
        var builder = Builders<BsonDocument>.Filter;
        var filters = new List<FilterDefinition<BsonDocument>>();

        foreach (var filterIn in filtersIn)
        {
            switch (filterIn.Operation)
            {
                case (char)Operations.Equals:
                    filters.Add(builder.Eq(filterIn.Key, filterIn.Value));
                    break;
                case (char)Operations.GreaterThan:
                    filters.Add(builder.Gt(filterIn.Key, filterIn.Value));
                    break;
                case (char)Operations.LessThan:
                    filters.Add(builder.Lt(filterIn.Key, filterIn.Value));
                    break;
                case (char)Operations.NotEquals:
                    filters.Add(builder.Not(builder.Eq(filterIn.Key, filterIn.Value)));
                    break;
                case (char)Operations.Contains:
                    filters.Add(
                        builder.Regex(filterIn.Key,
                            new BsonRegularExpression($".*{filterIn.Value}.*", "i")));
                    break;
            }
        }

        return filters.Count > 0 ? builder.And(filters) : builder.Empty;
    }
}
