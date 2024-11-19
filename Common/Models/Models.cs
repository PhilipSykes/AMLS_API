namespace Common.Models
{
    public class ReserveRequest
    {
        public string UserId { get; init; } = string.Empty;
        public string MediaID { get; init; } = string.Empty; //Object ID
        public Dictionary<string,string> EmailDetails { get; init; } = new Dictionary<string, string>();
    }



    public class LoginRequest
    {
        public string UserId { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public Dictionary<string,string> EmailDetails { get; init; } = new Dictionary<string, string>();
    }
}