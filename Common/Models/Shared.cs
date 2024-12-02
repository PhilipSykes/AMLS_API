using Common.Constants;

namespace Common.Models;

public static class Shared
{
    public record EmailDetails
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> RecipientAddresses { get; set; } = new();
        public Dictionary<string, string> EmailBody { get; set; } = new();
    }

    public record Filter
    {
        public Filter(string key, string value, DbOperations operation)
        {
            Key = key;
            Value = value;
            Operation = operation;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public DbOperations Operation { get; set; }
    }

    public record LoginDetails
    {
        public string? Username { get; set; }
        public string? Token { get; set; }
    }
}