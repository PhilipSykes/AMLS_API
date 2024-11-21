namespace Common.Models;

public static class Operations
{
    public record Request<T>
    {
        public T? Data { get; init; }
        public EmailDetails? EmailDetails { get; init; }
        public List<Filter>? SearchFilters { get; init; }

    }

    public record Response<T>
    {
        public T? Data { get; init; }
        public bool Success { get; init; }
        public string? Error { get; init; }
    }

}