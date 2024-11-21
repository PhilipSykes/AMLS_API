

using Common.Constants;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace Common.Models
{
    public class EmailDetails
    {
        public string UserId { get; init; } = string.Empty;
        public List<string> RecipientAddresses { get; init; } = new List<string>();
        public Dictionary<string,string> EmailBody { get; init; } = new Dictionary<string, string>();
    }

    public class User
    {
        public string Id { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PasswordHash { get; init; } = string.Empty;
    }
    
    public class ReserveRequest
    {
        public string ObjectId { get; init; } = string.Empty;
        public string UserId { get; init; } = string.Empty;
        public EmailDetails EmailDetails { get; init; } = new EmailDetails();
    }


    public class LoginRequest
    {
        public string UserId { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public EmailDetails EmailDetails { get; init; } = new EmailDetails();
    }
   
    public class Filter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DbOperations Operation { get; set; }

        public Filter(string key, string value, DbOperations operation)
        {
            Key = key;
            Value = value;
            Operation = operation;
        }
    }

    public class SearchResponse
    {
        public List<string> Results { get; set; } = new();
        public int TotalCount { get; set; }
        public string? Error { get; set; }
    }
}

