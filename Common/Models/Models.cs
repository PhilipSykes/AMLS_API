namespace Common.Models
{
    public class EmailDetails
    {
        public string UserId { get; init; } = string.Empty;
        public List<string> RecipientAddresses { get; init; } = new List<string>();
        public Dictionary<string,string> EmailBody { get; init; } = new Dictionary<string, string>();
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
}