namespace Common.Models;

public class Models
{
    public record LoginRequest
    {
        public string UserId { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
    
    public record SearchFilters
    {
        public string UserId { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
    
    
}

