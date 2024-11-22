namespace Common.Models;

public static class PayLoads
    {
        public record Login
        {
            public required string Email { get; init; }
            public required string Password { get; init; }
        }

        public record Reserve
        {
            public required string MediaId { get; init; }
            public required string UserId { get; init; }
        }
    }