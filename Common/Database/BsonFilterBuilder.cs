using Common.Constants;
using Common.Exceptions;
using MongoDB.Bson;
using MongoDB.Driver;
using static Common.Models.Shared;

namespace Common.Database;

public interface IFilterBuilder<T>
{
    FilterDefinition<T> BuildFilter(List<Filter> filterObjectsIn);
}
public class BsonFilterBuilder : IFilterBuilder<BsonDocument>
{
    public FilterDefinition<BsonDocument> BuildFilter(List<Filter> filterObjectsIn)
    {
        try // Note from Will: This try catch is redundant, as operators are enforced by the enum. // Second note from Will: Keep for now, Add AND/OR to filters
        {
            var builder = Builders<BsonDocument>.Filter;

            if (filterObjectsIn is null) // Allows filterless queries
                return builder.Empty;


            var mongoFilters = Builders<BsonDocument>.Filter.Empty;

            foreach (var filterObject in filterObjectsIn)
                switch (filterObject.Operation)
                {
                    case FilterTypes.Equals:
                        if (filterObject.Key == "_id")
                            mongoFilters &= builder.Eq(filterObject.Key, new ObjectId(filterObject.Value));
                        else
                            mongoFilters &= builder.Eq(filterObject.Key, filterObject.Value);
                        break;
                    case FilterTypes.GreaterThan:
                        mongoFilters &= builder.Gt(filterObject.Key, filterObject.Value);
                        break;
                    case FilterTypes.LessThan:
                        mongoFilters &= builder.Lt(filterObject.Key, filterObject.Value);
                        break;
                    case FilterTypes.NotEquals:
                        mongoFilters &= builder.Not(builder.Eq(filterObject.Key, filterObject.Value));
                        break;
                    case FilterTypes.Contains:
                        if (filterObject.Value is string) // This is a temp hack, fix properly later
                            mongoFilters &= builder.Regex(filterObject.Key,
                                new BsonRegularExpression($".*{filterObject.Value}.*", "i"));
                        else
                            mongoFilters &= builder.AnyEq(filterObject.Key, (string)filterObject.Value);

                        break;
                }

            return mongoFilters;
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
        {
            throw new SearchException(SearchException.SearchErrorType.Validation);
        }
    }
}