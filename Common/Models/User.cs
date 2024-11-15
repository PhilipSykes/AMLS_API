namespace Common.Models;

public class User
{
    
}

public record LoginRequest
{
    public string UserId { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}