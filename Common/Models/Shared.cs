using Common.Constants;


namespace Common.Models
{
    public class EmailDetails
    {
        public string UserId { get; init; } = string.Empty;
        public List<string> RecipientAddresses { get; init; } = new List<string>();
        public Dictionary<string, string> EmailBody { get; init; } = new Dictionary<string, string>();
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
}


