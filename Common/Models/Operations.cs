namespace Common.Models;

public static class Operations
{
    public record Request<T>
    {
        public T? Data { get; set; }
        public EmailDetails? EmailDetails { get; set; }
        public List<Filter>? SearchFilters { get; set; }

    }

    public record Response<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

}