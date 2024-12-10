using Common.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Common.Utils;

public static class BsonDTOMapper
{
    public static List<T> ConvertBsonToEntity<T>(List<BsonDocument> bsonDocuments)
    {
        try
        {
            return bsonDocuments
                .Select(doc => BsonSerializer.Deserialize<T>(doc))
                .ToList();
        }
        catch (BsonSerializationException)
        {
            throw new SearchException(SearchException.SearchErrorType.Serialization);
        }
    }
}