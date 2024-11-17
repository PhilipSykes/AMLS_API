using MongoDB.Bson;

namespace Common.Models
{
    
    public class LoginRequest
    {
        public string UserId { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
    
    public class Filter
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public char Operation { get; set; }

        public Filter(string key, object value, char operation)
        {
            Key = key;
            Value = value;
            Operation = operation;
        }
    }

    public class SearchResponse
    {
        public List<BsonDocument> Results { get; set; } = new();
        public int TotalCount { get; set; }
        public string? Error { get; set; }
    }
}



