using Common.Constants;


namespace Common.Models
{
    public static class Shared
    {
        public record EmailDetails
        {
            public string UserId { get; set; } = string.Empty;
            public List<string> RecipientAddresses { get; set; } = new List<string>();
            public Dictionary<string, string> EmailBody { get; set; } = new Dictionary<string, string>();
        }

        public record Filter
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public FilterTypes Operation { get; set; }

            public Filter(string key, string value, FilterTypes operation)
            {
                Key = key;
                Value = value;
                Operation = operation;
            }
        }
        
        public record LoginDetails
        {
            public string? UserID { get; set; }
            public string? Token { get; set; }
        }
    }
}


