namespace Common.Models;

public static class PayLoads
    {
        public record Login
        {
            public required string Username { get; init; }
            public required string Password { get; init; }
        }

        public record Reservation
        {
            public required string MediaId { get; init; }
            public required string UserId { get; init; }
        }
    }