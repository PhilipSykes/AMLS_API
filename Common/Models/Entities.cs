namespace Common.Models;

public static class Entities
{
    public record User
    {
        public string Id { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PasswordHash { get; init; } = string.Empty;
    }
    
    
}