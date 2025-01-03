using System.ComponentModel.DataAnnotations;
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
            [Required]
            [RegularExpression(@"^[a-zA-Z0-9_]*$")]
            public string Key { get; set; }
            [Required]
            [RegularExpression(@"^[a-zA-Z0-9_]*$")]
            public string Value { get; set; }
            public DbEnums Operation { get; set; }
            public bool IsPostLookup { get; set; }
            
            public bool IsObjectId { get; set; }

            public Filter(string key, string value, DbEnums operation)
            {
                Key = key;
                Value = value;
                Operation = operation;
                IsPostLookup = false;
                IsObjectId = false;
            }
        }
        
        public record LoginDetails
        {
            [Required]
            [RegularExpression(@"^[a-zA-Z0-9_]*$")]
            public string? UserID { get; set; }
            [Required]
            [RegularExpression(@"^[a-zA-Z0-9_]*$")]
            public string[] Branches { get; set; } = [];
            public string? Token { get; set; }
        }
        
        public class AgreggateSearchConfig
        {
            public bool UseAggregation { get; set; } = false;
            public List<string> LookupCollections { get; set; } = new();
            public List<string> LocalFields { get; set; } = new();
            public List<string> ForeignFields { get; set; } = new();
            public List<string> OutputFields { get; set; } = new(); 
            public string ProjectionString { get; set; }
        }

        public class GetReservablesRequest
        {
            public string Media { get; set; }
            public string[] Branches { get; set; }
            public int MinimumDays { get; set; }
        }
        
    }
}


