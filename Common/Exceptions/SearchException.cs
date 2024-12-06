namespace Common.Exceptions;

public class SearchException : Exception
{
    public enum SearchErrorType
    {
        Database,
        Serialization,
        Validation,
        Unknown
    }

    private static readonly Dictionary<SearchErrorType, string> Messages = new()
    {
        { SearchErrorType.Database, "Database operation failed" },
        { SearchErrorType.Serialization, "Failed to process search results" },
        { SearchErrorType.Validation, "Invalid search parameters" },
        { SearchErrorType.Unknown, "An unexpected error occurred" }
    };

    public SearchException(SearchErrorType errorType)
        : base(Messages[errorType])
    {
        ErrorType = errorType;
    }

    public SearchErrorType ErrorType { get; }
}